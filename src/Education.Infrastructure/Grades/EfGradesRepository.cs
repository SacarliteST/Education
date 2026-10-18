using Education.Application.Grades;
using Education.Domain.Practicals;
using Education.Domain.PracticalModules;
using Education.Infrastructure.Persistence;
using Education.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Grades;

internal sealed class EfGradesRepository(EducationDbContext context) : IGradesRepository
{
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

        var studentIds = await context.PracticalBindUsers
            .Where(bind => bind.PracticalMaterialId == practicalId)
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

    public async Task<Gradebook> GetCourseGradebookAsync(Guid courseId, CancellationToken cancellationToken = default)
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
        var practicalIds = practicals.Select(item => item.Practical.Id).ToList();

        var assignments = await context.PracticalBindUsers
            .Where(bind => practicalIds.Contains(bind.PracticalMaterialId))
            .Select(bind => new { bind.PracticalMaterialId, bind.UserId })
            .ToListAsync(cancellationToken);
        var courseStudentIds = await context.CourseBindUsers
            .Where(bind => bind.CourseId == courseId)
            .Select(bind => bind.UserId)
            .ToListAsync(cancellationToken);
        var studentIds = courseStudentIds.Concat(assignments.Select(item => item.UserId)).Distinct().ToList();
        var people = await LoadPeopleAsync(studentIds, cancellationToken);

        var assignedByPractical = assignments
            .GroupBy(item => item.PracticalMaterialId)
            .ToDictionary(group => group.Key, group => group.Select(item => item.UserId).ToHashSet());
        var gradesByPractical = new Dictionary<Guid, IReadOnlyDictionary<Guid, PracticalGrade>>();
        foreach (var item in practicals)
        {
            var assigned = assignedByPractical.GetValueOrDefault(item.Practical.Id) ?? [];
            gradesByPractical[item.Practical.Id] = assigned.Count == 0
                ? new Dictionary<Guid, PracticalGrade>()
                : await CalculateAsync(item.Practical, assigned.ToList(), cancellationToken);
        }

        var students = studentIds
            .Select(id => new GradebookStudent(
                id,
                people[id].Name,
                people[id].Login,
                people[id].Group,
                practicals.Select(item =>
                {
                    var assigned = assignedByPractical.GetValueOrDefault(item.Practical.Id)?.Contains(id) == true;
                    return new GradebookCell(
                        item.Practical.Id,
                        assigned,
                        assigned ? gradesByPractical[item.Practical.Id][id] : null);
                }).ToList()))
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
        CancellationToken cancellationToken)
    {
        if (practical.Kind == PracticalKind.External)
        {
            var bestScores = (await context.PracticalModuleSessions
                    .Where(session => studentIds.Contains(session.UserId)
                        && session.Status == ModuleSessionState.Completed
                        && session.Grade != null
                        && context.Cases.Any(task =>
                            task.Id == session.PracticalTaskId && task.PracticalMaterialId == practical.Id))
                    .GroupBy(session => session.UserId)
                    .Select(group => new { UserId = group.Key, Best = group.Max(session => session.Grade) })
                    .ToListAsync(cancellationToken))
                .ToDictionary(item => item.UserId, item => item.Best);

            return studentIds.ToDictionary(
                id => id,
                id => PracticalGradeCalculator.External(practical, bestScores.GetValueOrDefault(id)));
        }

        var totalTasks = await context.Cases.CountAsync(task => task.PracticalMaterialId == practical.Id, cancellationToken);
        var files = await context.CaseFiles
            .Where(file => file.Case.PracticalMaterialId == practical.Id && studentIds.Contains(file.UserId))
            .Select(file => new { file.UserId, file.IsAccepted, file.Grade })
            .ToListAsync(cancellationToken);
        var results = await context.TestResults
            .AsNoTracking()
            .Where(result => result.PracticalMaterialId == practical.Id
                && studentIds.Contains(result.UserId)
                && result.IsCompleted)
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
}
