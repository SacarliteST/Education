using Education.Application.ClassWindows;
using Education.Application.PracticalModules;
using Education.Domain.Courses;
using Education.Domain.Practicals;
using Microsoft.Extensions.Logging;

namespace Education.Application.Practicals.TaskAssignments;

/// <summary>
/// Формирование персональных случайных наборов заданий модуля для студентов
/// по правилам выборки практик курса.
/// </summary>
public sealed class TaskAssignmentService : ITaskAssignmentService
{
    private readonly IClassWindowsRepository classWindowsRepository;
    private readonly ITaskSelectionRulesRepository rulesRepository;
    private readonly IPracticalTaskAssignmentsRepository assignmentsRepository;
    private readonly IModuleCatalogService moduleCatalogService;
    private readonly TimeProvider timeProvider;
    private readonly ILogger<TaskAssignmentService> logger;

    /// <summary>Создаёт экземпляр класса <see cref="TaskAssignmentService"/>.</summary>
    /// <param name="classWindowsRepository">Хранилище пар (аудитория пары).</param>
    /// <param name="rulesRepository">Хранилище правил выборки.</param>
    /// <param name="assignmentsRepository">Хранилище назначений заданий.</param>
    /// <param name="moduleCatalogService">Каталог заданий внешних модулей.</param>
    /// <param name="timeProvider">Источник текущего времени.</param>
    /// <param name="logger">Журнал.</param>
    public TaskAssignmentService(
        IClassWindowsRepository classWindowsRepository,
        ITaskSelectionRulesRepository rulesRepository,
        IPracticalTaskAssignmentsRepository assignmentsRepository,
        IModuleCatalogService moduleCatalogService,
        TimeProvider timeProvider,
        ILogger<TaskAssignmentService> logger)
    {
        this.classWindowsRepository = classWindowsRepository;
        this.rulesRepository = rulesRepository;
        this.assignmentsRepository = assignmentsRepository;
        this.moduleCatalogService = moduleCatalogService;
        this.timeProvider = timeProvider;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task GenerateForWindowAsync(Guid classWindowId, CancellationToken cancellationToken = default)
    {
        var window = await classWindowsRepository.GetByIdAsync(classWindowId, cancellationToken);
        if (window is null)
        {
            logger.LogWarning("Генерация наборов пропущена: пара {WindowId} не найдена.", classWindowId);
            return;
        }

        var rules = await rulesRepository.ListForCourseAsync(window.CourseId, cancellationToken);
        if (rules.Count == 0)
        {
            return;
        }

        var audience = await ResolveAudienceAsync(window, cancellationToken);
        if (audience.Count == 0)
        {
            return;
        }

        foreach (var rule in rules)
        {
            var candidates = await LoadCandidatesAsync(rule, cancellationToken);
            if (candidates is not { Count: > 0 })
            {
                continue;
            }

            var existing = await assignmentsRepository.ListAsync(rule.PracticalMaterialId, window.Id, cancellationToken);
            var pending = audience
                .Where(studentId => existing.All(assignment => assignment.StudentId != studentId))
                .ToList();
            if (pending.Count == 0)
            {
                continue;
            }

            var now = timeProvider.GetUtcNow();
            var created = pending
                .SelectMany(studentId => BuildSet(rule, window.Id, studentId, candidates, now))
                .ToList();
            await assignmentsRepository.AddRangeAsync(created, cancellationToken);
            await assignmentsRepository.SaveChangesAsync(cancellationToken);
            logger.LogInformation(
                "Сформированы наборы заданий по практике {PracticalId} на пару {WindowId}: студентов {StudentsCount}, заданий каждому до {TaskCount}.",
                rule.PracticalMaterialId, window.Id, pending.Count, rule.TaskCount);
        }
    }

    /// <inheritdoc />
    public async Task EnsureForStudentAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default)
    {
        if (await assignmentsRepository.ExistsForStudentAsync(practicalId, classWindowId, studentId, cancellationToken))
        {
            return;
        }

        var rule = await rulesRepository.GetByPracticalIdAsync(practicalId, cancellationToken);
        if (rule is null)
        {
            return;
        }

        var candidates = await LoadCandidatesAsync(rule, cancellationToken);
        if (candidates is not { Count: > 0 })
        {
            return;
        }

        var created = BuildSet(rule, classWindowId, studentId, candidates, timeProvider.GetUtcNow());
        await assignmentsRepository.AddRangeAsync(created, cancellationToken);
        await assignmentsRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Лениво сформирован набор заданий по практике {PracticalId} для студента {StudentId} на пару {WindowId}.",
            practicalId, studentId, classWindowId);
    }

    /// <inheritdoc />
    public async Task RegenerateForStudentAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default)
    {
        if (await assignmentsRepository.HasSessionsForStudentAsync(practicalId, classWindowId, studentId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в пересборе набора по практике {PracticalId} для студента {StudentId}: уже есть сессии модуля.",
                practicalId, studentId);
            throw new AssignmentRegenerationForbiddenException();
        }

        await assignmentsRepository.DeleteForStudentAsync(practicalId, classWindowId, studentId, cancellationToken);
        await EnsureForStudentAsync(practicalId, classWindowId, studentId, cancellationToken);
        logger.LogInformation(
            "Набор заданий студента {StudentId} по практике {PracticalId} на пару {WindowId} пересобран.",
            studentId, practicalId, classWindowId);
    }

    private async Task<IReadOnlyList<Guid>> ResolveAudienceAsync(ClassWindow window, CancellationToken cancellationToken)
    {
        var courseStudents = await classWindowsRepository.ListStudentsAsync(window.CourseId, cancellationToken);
        var group = window.GroupName?.Trim();
        return courseStudents
            .Where(student =>
                (group is not null && String.Equals(student.GroupName?.Trim(), group, StringComparison.OrdinalIgnoreCase))
                || window.Students.Any(bound => bound.UserId == student.UserId))
            .Select(student => student.UserId)
            .ToList();
    }

    private async Task<IReadOnlyList<ModuleCatalogTask>?> LoadCandidatesAsync(
        TaskSelectionRule rule, CancellationToken cancellationToken)
    {
        IReadOnlyList<ModuleCatalogTask> tasks;
        try
        {
            tasks = await moduleCatalogService.GetTasksAsync(rule.PracticalModuleId, cancellationToken) ?? [];
        }
        catch (ModuleCatalogUnavailableException exception)
        {
            logger.LogWarning(
                exception,
                "Каталог модуля {ModuleId} недоступен: генерация наборов по практике {PracticalId} отложена до ленивой догенерации.",
                rule.PracticalModuleId, rule.PracticalMaterialId);
            return null;
        }

        var candidates = tasks.Where(task => TaskSelectionFilter.Matches(rule, task)).ToList();
        if (candidates.Count == 0)
        {
            logger.LogWarning(
                "Правило выборки практики {PracticalId} дало 0 заданий: в каталоге модуля {ModuleId} нет подходящих.",
                rule.PracticalMaterialId, rule.PracticalModuleId);
        }

        return candidates;
    }

    private static IReadOnlyList<PracticalTaskAssignment> BuildSet(
        TaskSelectionRule rule,
        Guid classWindowId,
        Guid studentId,
        IReadOnlyList<ModuleCatalogTask> candidates,
        DateTimeOffset now)
    {
        return PickRandom(candidates, rule.TaskCount)
            .Select((task, index) => new PracticalTaskAssignment(
                rule.PracticalMaterialId, studentId, classWindowId, task.Ref, index, now))
            .ToList();
    }

    private static List<ModuleCatalogTask> PickRandom(IReadOnlyList<ModuleCatalogTask> candidates, int count)
    {
        if (candidates.Count <= count)
        {
            return candidates.ToList();
        }

        return candidates
            .OrderBy(_ => Random.Shared.Next())
            .Take(count)
            .ToList();
    }
}
