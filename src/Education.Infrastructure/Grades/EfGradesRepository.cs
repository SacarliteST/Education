using Education.Application.ClassWindows;
using Education.Application.Grades;
using Education.Domain.PracticalModules;
using Education.Domain.Practicals;
using Education.Infrastructure.Persistence;
using Education.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Grades;

/// <summary>EF Core-реализация хранилища данных для расчёта оценок.</summary>
internal sealed class EfGradesRepository : IGradesRepository
{
    private readonly EducationDbContext context;
    private readonly TimeProvider timeProvider;

    /// <summary>Создаёт экземпляр класса <see cref="EfGradesRepository"/>.</summary>
    /// <param name="context">Контекст базы данных Education.</param>
    /// <param name="timeProvider">Источник текущего времени.</param>
    public EfGradesRepository(EducationDbContext context, TimeProvider timeProvider)
    {
        this.context = context;
        this.timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public async Task<PracticalGrade> GetPracticalGradeAsync(
        Guid practicalId,
        Guid studentUserId,
        CancellationToken cancellationToken = default)
    {
        var practicalMaterial = await context.PracticalMaterials
            .AsNoTracking()
            .FirstOrDefaultAsync(practical => practical.Id == practicalId, cancellationToken);
        if (practicalMaterial is null)
        {
            // Как и раньше: несуществующая практика считается внутренней и даёт «Пройдите тестирование».
            return PracticalGradeCalculator.Internal(0, 0, [], []);
        }

        var grades = await CalculateAsync(practicalMaterial, [studentUserId], cancellationToken);
        return grades[studentUserId];
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StudentPracticalGrade>> GetPracticalStudentGradesAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        var practicalMaterial = await context.PracticalMaterials
            .AsNoTracking()
            .FirstOrDefaultAsync(practical => practical.Id == practicalId, cancellationToken);
        if (practicalMaterial is null)
        {
            return [];
        }

        // Курс практики достаём отдельным запросом: сущность загружена без Include(Module).
        var courseId = await context.Modules
            .Where(module => module.Id == practicalMaterial.ModuleId)
            .Select(module => module.CourseId)
            .FirstAsync(cancellationToken);
        var studentIds = await context.CourseBindUsers
            .Where(bind => bind.CourseId == courseId)
            .Select(bind => bind.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);
        var people = await LoadPeopleAsync(studentIds, cancellationToken);
        var grades = await CalculateAsync(practicalMaterial, studentIds, cancellationToken);

        return studentIds
            .Select(id => new StudentPracticalGrade(id, people[id].Name, people[id].Login, people[id].Group, grades[id]))
            .OrderBy(row => row.StudentName, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<Gradebook> GetCourseGradebookAsync(
        Guid courseId,
        ClassWindowResultsScope? classWindowScope = null,
        CancellationToken cancellationToken = default)
    {
        var practicals = await (
                from practical in context.PracticalMaterials.AsNoTracking()
                join module in context.Modules.AsNoTracking() on practical.ModuleId equals module.Id
                where module.CourseId == courseId
                select new { Practical = practical, ModuleName = module.Name })
            .ToListAsync(cancellationToken);
        practicals = practicals
            .OrderBy(item => item.ModuleName, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(item => item.Practical.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
        // Назначение единое — на курс: в ведомости все студенты курса по всем его практикам.
        var studentIds = await context.CourseBindUsers
            .Where(bind => bind.CourseId == courseId)
            .Select(bind => bind.UserId)
            .ToListAsync(cancellationToken);
        var people = await LoadPeopleAsync(studentIds, cancellationToken);

        var gradesByPractical = new Dictionary<Guid, IReadOnlyDictionary<Guid, PracticalGrade>>();
        foreach (var item in practicals)
        {
            gradesByPractical[item.Practical.Id] = studentIds.Count == 0
                ? new Dictionary<Guid, PracticalGrade>()
                : await CalculateAsync(item.Practical, studentIds, cancellationToken, classWindowScope);
        }

        var students = studentIds
            .Select(id => new GradebookStudent(
                id,
                people[id].Name,
                people[id].Login,
                people[id].Group,
                practicals.Select(item => new GradebookCell(
                    item.Practical.Id,
                    Assigned: true,
                    gradesByPractical[item.Practical.Id][id])).ToList()))
            .OrderBy(student => student.StudentName, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        return new Gradebook(
            practicals
                .Select(item => new GradebookPractical(
                    item.Practical.Id,
                    item.Practical.Name,
                    item.Practical.Kind == PracticalKind.External,
                    item.Practical.ModuleId,
                    item.ModuleName))
                .ToList(),
            students);
    }

    /// <inheritdoc />
    public async Task<StudentPracticalResultsAccess?> GetStudentPracticalResultsAccessAsync(
        Guid practicalId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var practical = await context.PracticalMaterials
            .AsNoTracking()
            .Where(item => item.Id == practicalId)
            .Select(item => new
            {
                item.Module.CourseId,
                TeacherId = item.Module.Course.UserId,
                IsStudentAssigned = item.Module.Course.CourseBindUsers.Any(bind => bind.UserId == studentId),
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (practical is null)
        {
            return null;
        }

        var student = await context.Users
            .AsNoTracking()
            .Where(user => user.Id == studentId)
            .Select(user => new { user.LastName, user.FirstName, user.MiddleName })
            .FirstOrDefaultAsync(cancellationToken);
        var studentName = student is null
            ? studentId.ToString()
            : PersonNameFormatter.Format(student.LastName, student.FirstName, student.MiddleName);

        return new StudentPracticalResultsAccess(
            practical.CourseId,
            practical.TeacherId,
            practical.IsStudentAssigned,
            studentName);
    }

    /// <inheritdoc />
    public async Task<StudentPracticalResults> GetStudentPracticalResultsAsync(
        Guid practicalId,
        Guid studentId,
        string studentName,
        ClassWindowResultsScope? classWindowScope = null,
        CancellationToken cancellationToken = default)
    {
        var kind = await context.PracticalMaterials
            .AsNoTracking()
            .Where(practical => practical.Id == practicalId)
            .Select(practical => practical.Kind)
            .SingleAsync(cancellationToken);

        if (kind == PracticalKind.Internal)
        {
            var attemptsQuery = context.TestResults
                .AsNoTracking()
                .Where(result => result.PracticalMaterialId == practicalId && result.UserId == studentId);
            if (classWindowScope is not null)
            {
                var startsAt = classWindowScope.StartsAt.UtcDateTime;
                var endsAt = classWindowScope.EndsAt.UtcDateTime;
                attemptsQuery = attemptsQuery.Where(result =>
                    result.StatedDate >= startsAt && result.StatedDate <= endsAt);
            }

            var attemptRows = await attemptsQuery
                .OrderByDescending(result => result.StatedDate)
                .Select(result => new
                {
                    result.Id,
                    result.TryNumber,
                    result.Score,
                    result.MaxScore,
                    result.StatedDate,
                    result.TurnedDate,
                    result.IsCompleted,
                })
                .ToListAsync(cancellationToken);
            var attempts = attemptRows.Select(result => new StudentTestAttemptResult(
                    result.Id,
                    result.TryNumber,
                    result.Score,
                    result.MaxScore,
                    ToUtcOffset(result.StatedDate),
                    result.TurnedDate == null ? null : ToUtcOffset(result.TurnedDate.Value),
                    result.IsCompleted))
                .ToList();

            return new StudentPracticalResults(
                studentId,
                studentName,
                practicalId,
                kind,
                classWindowScope?.ClassWindowId,
                null,
                [],
                attempts);
        }

        var selectionModuleId = await context.TaskSelectionRules
            .AsNoTracking()
            .Where(rule => rule.PracticalMaterialId == practicalId)
            .Select(rule => (Guid?)rule.PracticalModuleId)
            .FirstOrDefaultAsync(cancellationToken);
        var tasks = selectionModuleId is not null
            ? await LoadAssignedTaskResultsAsync(practicalId, studentId, classWindowScope, cancellationToken)
            : await LoadFixedTaskResultsAsync(practicalId, studentId, classWindowScope, cancellationToken);
        var practicalModuleId = selectionModuleId ?? await context.Cases
            .AsNoTracking()
            .Where(task => task.PracticalMaterialId == practicalId && task.PracticalModuleId != null)
            .Select(task => task.PracticalModuleId)
            .FirstOrDefaultAsync(cancellationToken);

        return new StudentPracticalResults(
            studentId,
            studentName,
            practicalId,
            kind,
            classWindowScope?.ClassWindowId,
            practicalModuleId,
            tasks,
            []);
    }

    private async Task<IReadOnlyList<StudentPracticalTaskResult>> LoadAssignedTaskResultsAsync(
        Guid practicalId,
        Guid studentId,
        ClassWindowResultsScope? classWindowScope,
        CancellationToken cancellationToken)
    {
        var assignmentsQuery = context.PracticalTaskAssignments
            .AsNoTracking()
            .Where(assignment => assignment.PracticalMaterialId == practicalId && assignment.StudentId == studentId);
        if (classWindowScope is not null)
        {
            assignmentsQuery = assignmentsQuery.Where(assignment =>
                assignment.ClassWindowId == classWindowScope.ClassWindowId);
        }

        var assignments = await assignmentsQuery
            .OrderByDescending(assignment => assignment.CreatedAt)
            .ThenBy(assignment => assignment.SortOrder)
            .Select(assignment => new
            {
                assignment.Id,
                assignment.SortOrder,
                assignment.ExternalTaskRef,
                assignment.ClassWindowId,
            })
            .ToListAsync(cancellationToken);
        var assignmentIds = assignments.Select(assignment => assignment.Id).ToList();
        var sessions = await context.PracticalModuleSessions
            .AsNoTracking()
            .Where(session => session.AssignmentId != null && assignmentIds.Contains(session.AssignmentId.Value))
            .OrderByDescending(session => session.StartedAt)
            .ToListAsync(cancellationToken);

        return assignments.Select(assignment => new StudentPracticalTaskResult(
            assignment.Id,
            null,
            assignment.SortOrder,
            assignment.ExternalTaskRef,
            null,
            assignment.ClassWindowId,
            sessions.Where(session => session.AssignmentId == assignment.Id).Select(ToResult).ToList()))
            .ToList();
    }

    private async Task<IReadOnlyList<StudentPracticalTaskResult>> LoadFixedTaskResultsAsync(
        Guid practicalId,
        Guid studentId,
        ClassWindowResultsScope? classWindowScope,
        CancellationToken cancellationToken)
    {
        var fixedTasks = await context.Cases
            .AsNoTracking()
            .Where(task => task.PracticalMaterialId == practicalId && task.ExternalTaskRef != null)
            .OrderBy(task => task.Name)
            .ThenBy(task => task.Id)
            .Select(task => new { task.Id, ExternalTaskRef = task.ExternalTaskRef! })
            .ToListAsync(cancellationToken);
        var taskIds = fixedTasks.Select(task => task.Id).ToList();
        var sessionsQuery = context.PracticalModuleSessions
            .AsNoTracking()
            .Where(session => session.UserId == studentId
                && session.PracticalTaskId != null
                && taskIds.Contains(session.PracticalTaskId.Value));
        if (classWindowScope is not null)
        {
            sessionsQuery = sessionsQuery.Where(session =>
                session.StartedAt >= classWindowScope.StartsAt && session.StartedAt <= classWindowScope.EndsAt);
        }

        var sessions = await sessionsQuery
            .OrderByDescending(session => session.StartedAt)
            .ToListAsync(cancellationToken);

        return fixedTasks.Select((task, index) => new StudentPracticalTaskResult(
            null,
            task.Id,
            index,
            task.ExternalTaskRef,
            null,
            null,
            sessions.Where(session => session.PracticalTaskId == task.Id).Select(ToResult).ToList()))
            .ToList();
    }

    private static StudentPracticalSessionResult ToResult(PracticalModuleSession session) => new(
        session.Id,
        session.Status,
        session.EndReason,
        session.Grade,
        session.StartedAt,
        session.EndedAt);

    private static DateTimeOffset ToUtcOffset(DateTime value) =>
        new(DateTime.SpecifyKind(value, DateTimeKind.Utc));

    private async Task<Dictionary<Guid, (string Name, string Login, string? Group)>> LoadPeopleAsync(
        IReadOnlyCollection<Guid> userIds,
        CancellationToken cancellationToken)
    {
        var users = await context.Users.AsNoTracking()
            .Where(user => userIds.Contains(user.Id))
            .Select(user => new { user.Id, user.LastName, user.FirstName, user.MiddleName, user.Login, user.GroupName })
            .ToListAsync(cancellationToken);
        var result = users.ToDictionary(
            user => user.Id,
            user => (PersonNameFormatter.Format(user.LastName, user.FirstName, user.MiddleName), user.Login, user.GroupName));

        // Пользователь мог быть удалён — не роняем ведомость, показываем прочерк.
        foreach (var id in userIds.Where(id => !result.ContainsKey(id)))
        {
            result[id] = ("—", String.Empty, null);
        }

        return result;
    }

    /// <summary>Оценки студентов по одной практике — одно правило для одного и для многих студентов.</summary>
    private async Task<Dictionary<Guid, PracticalGrade>> CalculateAsync(
        PracticalMaterial practical,
        IReadOnlyCollection<Guid> studentIds,
        CancellationToken cancellationToken,
        ClassWindowResultsScope? classWindowScope = null)
    {
        if (practical.Kind == PracticalKind.External)
        {
            var usesRandomTaskSet = await context.TaskSelectionRules
                .AsNoTracking()
                .AnyAsync(rule => rule.PracticalMaterialId == practical.Id, cancellationToken);
            if (usesRandomTaskSet)
            {
                return await CalculateAssignedTaskSetGradesAsync(
                    practical,
                    studentIds,
                    classWindowScope,
                    cancellationToken);
            }

            var sessions = context.PracticalModuleSessions
                .Where(session => studentIds.Contains(session.UserId)
                    && session.Status == ModuleSessionState.Completed
                    && session.Grade != null);
            if (classWindowScope is null)
            {
                sessions = sessions.Where(session =>
                    context.Cases.Any(task =>
                        task.Id == session.PracticalTaskId && task.PracticalMaterialId == practical.Id)
                    || context.PracticalTaskAssignments.Any(assignment =>
                        assignment.Id == session.AssignmentId && assignment.PracticalMaterialId == practical.Id));
            }
            else
            {
                sessions = sessions.Where(session =>
                    context.PracticalTaskAssignments.Any(assignment =>
                        assignment.Id == session.AssignmentId
                        && assignment.PracticalMaterialId == practical.Id
                        && assignment.ClassWindowId == classWindowScope.ClassWindowId)
                    || (session.StartedAt >= classWindowScope.StartsAt
                        && session.StartedAt <= classWindowScope.EndsAt
                        && context.Cases.Any(task =>
                            task.Id == session.PracticalTaskId && task.PracticalMaterialId == practical.Id)));
            }

            var bestScores = (await sessions
                    .GroupBy(session => session.UserId)
                    .Select(group => new { UserId = group.Key, Best = group.Max(session => session.Grade) })
                    .ToListAsync(cancellationToken))
                .ToDictionary(item => item.UserId, item => item.Best);

            return studentIds.ToDictionary(
                id => id,
                id => PracticalGradeCalculator.External(practical, bestScores.GetValueOrDefault(id)));
        }

        var totalTasks = classWindowScope is null
            ? await context.Cases.CountAsync(task => task.PracticalMaterialId == practical.Id, cancellationToken)
            : 0;
        var files = await context.CaseFiles
            .Where(file => classWindowScope == null
                && file.Case.PracticalMaterialId == practical.Id
                && studentIds.Contains(file.UserId))
            .Select(file => new { file.UserId, file.IsAccepted, file.Grade })
            .ToListAsync(cancellationToken);
        var testResults = context.TestResults
            .AsNoTracking()
            .Where(result => result.PracticalMaterialId == practical.Id
                && studentIds.Contains(result.UserId)
                && result.IsCompleted);
        if (classWindowScope is not null)
        {
            var startsAt = classWindowScope.StartsAt.UtcDateTime;
            var endsAt = classWindowScope.EndsAt.UtcDateTime;
            testResults = testResults.Where(result => result.StatedDate >= startsAt && result.StatedDate <= endsAt);
        }

        var results = await testResults
            .Select(result => new { result.UserId, result.Score, result.MaxScore })
            .ToListAsync(cancellationToken);

        return studentIds.ToDictionary(
            id => id,
            id => PracticalGradeCalculator.Internal(
                totalTasks,
                files.Count(file => file.UserId == id && file.IsAccepted),
                results.Where(result => result.UserId == id)
                    .Select(result => practical.CalculateTestGrade(result.Score, result.MaxScore))
                    .ToList(),
                files.Where(file => file.UserId == id).Select(file => file.Grade).ToList()));
    }

    private async Task<Dictionary<Guid, PracticalGrade>> CalculateAssignedTaskSetGradesAsync(
        PracticalMaterial practical,
        IReadOnlyCollection<Guid> studentIds,
        ClassWindowResultsScope? classWindowScope,
        CancellationToken cancellationToken)
    {
        var assignmentsQuery = context.PracticalTaskAssignments
            .AsNoTracking()
            .Where(assignment => assignment.PracticalMaterialId == practical.Id
                && studentIds.Contains(assignment.StudentId));
        if (classWindowScope is not null)
        {
            assignmentsQuery = assignmentsQuery.Where(assignment =>
                assignment.ClassWindowId == classWindowScope.ClassWindowId);
        }

        var assignments = await assignmentsQuery
            .Select(assignment => new
            {
                assignment.Id,
                assignment.StudentId,
                assignment.ClassWindowId,
                assignment.ClassWindow.EndsAt,
                assignment.ClassWindow.ClosedAt,
            })
            .ToListAsync(cancellationToken);
        var assignmentIds = assignments.Select(assignment => assignment.Id).ToList();
        var bestScores = assignmentIds.Count == 0
            ? new Dictionary<Guid, int?>()
            : (await context.PracticalModuleSessions
                .AsNoTracking()
                .Where(session => session.AssignmentId != null
                    && assignmentIds.Contains(session.AssignmentId.Value)
                    && session.Status == ModuleSessionState.Completed
                    && session.Grade != null)
                .GroupBy(session => session.AssignmentId!.Value)
                .Select(group => new
                {
                    AssignmentId = group.Key,
                    Score = group.Max(session => session.Grade),
                })
                .ToListAsync(cancellationToken))
                .ToDictionary(item => item.AssignmentId, item => item.Score);
        var now = timeProvider.GetUtcNow();
        var bestSetScores = assignments
            .GroupBy(assignment => new { assignment.StudentId, assignment.ClassWindowId })
            .Select(set =>
            {
                var isFinished = set.Any(assignment => assignment.ClosedAt is not null || assignment.EndsAt <= now);
                var scores = set.Select(assignment => bestScores.GetValueOrDefault(assignment.Id)).ToList();
                var score = scores.All(item => item is not null) || isFinished
                    ? (int?)Math.Round(scores.Average(item => item ?? 0), MidpointRounding.AwayFromZero)
                    : null;
                return new { set.Key.StudentId, Score = score };
            })
            .Where(set => set.Score is not null)
            .GroupBy(set => set.StudentId)
            .ToDictionary(group => group.Key, group => group.Max(set => set.Score));

        return studentIds.ToDictionary(
            id => id,
            id => PracticalGradeCalculator.External(practical, bestSetScores.GetValueOrDefault(id)));
    }
}
