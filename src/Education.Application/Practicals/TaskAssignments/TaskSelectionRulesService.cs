using Education.Application.ClassWindows;
using Education.Application.Courses;
using Education.Application.PracticalModules;
using Education.Application.TestResults;
using Education.Application.Users;
using Education.Domain.Practicals;
using Microsoft.Extensions.Logging;

namespace Education.Application.Practicals.TaskAssignments;

/// <summary>Сценарии правил случайной выборки заданий и наборов студентов.</summary>
public sealed class TaskSelectionRulesService : ITaskSelectionRulesService
{
    private readonly IEducationUserResolver userResolver;
    private readonly IPracticalsRepository practicalsRepository;
    private readonly IPracticalModulesRepository practicalModulesRepository;
    private readonly ITaskSelectionRulesRepository rulesRepository;
    private readonly IPracticalTaskAssignmentsRepository assignmentsRepository;
    private readonly IPracticalModuleSessionsRepository sessionsRepository;
    private readonly IClassWindowsRepository classWindowsRepository;
    private readonly IClassAccessService classAccess;
    private readonly IModuleCatalogService moduleCatalogService;
    private readonly ITaskAssignmentService taskAssignmentService;
    private readonly ILogger<TaskSelectionRulesService> logger;

    /// <summary>Создаёт экземпляр класса <see cref="TaskSelectionRulesService"/>.</summary>
    /// <param name="userResolver">Сопоставление пользователя Identity с учебным пользователем.</param>
    /// <param name="practicalsRepository">Хранилище практик.</param>
    /// <param name="practicalModulesRepository">Реестр внешних практических модулей.</param>
    /// <param name="rulesRepository">Хранилище правил выборки.</param>
    /// <param name="assignmentsRepository">Хранилище назначений заданий.</param>
    /// <param name="sessionsRepository">Хранилище сессий внешних модулей.</param>
    /// <param name="classWindowsRepository">Хранилище пар (имена студентов).</param>
    /// <param name="classAccess">Проверка доступа по парам.</param>
    /// <param name="moduleCatalogService">Каталог заданий внешних модулей.</param>
    /// <param name="taskAssignmentService">Формирование наборов заданий.</param>
    /// <param name="logger">Журнал.</param>
    public TaskSelectionRulesService(
        IEducationUserResolver userResolver,
        IPracticalsRepository practicalsRepository,
        IPracticalModulesRepository practicalModulesRepository,
        ITaskSelectionRulesRepository rulesRepository,
        IPracticalTaskAssignmentsRepository assignmentsRepository,
        IPracticalModuleSessionsRepository sessionsRepository,
        IClassWindowsRepository classWindowsRepository,
        IClassAccessService classAccess,
        IModuleCatalogService moduleCatalogService,
        ITaskAssignmentService taskAssignmentService,
        ILogger<TaskSelectionRulesService> logger)
    {
        this.userResolver = userResolver;
        this.practicalsRepository = practicalsRepository;
        this.practicalModulesRepository = practicalModulesRepository;
        this.rulesRepository = rulesRepository;
        this.assignmentsRepository = assignmentsRepository;
        this.sessionsRepository = sessionsRepository;
        this.classWindowsRepository = classWindowsRepository;
        this.classAccess = classAccess;
        this.moduleCatalogService = moduleCatalogService;
        this.taskAssignmentService = taskAssignmentService;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<TaskSelectionRuleInfo?> GetRuleAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);
        var rule = await rulesRepository.GetByPracticalIdAsync(practicalId, cancellationToken);
        return rule is null ? null : ToInfo(rule);
    }

    /// <inheritdoc />
    public async Task<UpsertTaskSelectionRuleResult> UpsertRuleAsync(
        Guid practicalId, UpsertTaskSelectionRuleCommand command, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);

        var practical = await practicalsRepository.GetByIdAsync(practicalId, cancellationToken);
        if (practical is null)
        {
            logger.LogWarning(
                "Отклонено правило выборки практики {PracticalId}: практика не найдена.", practicalId);
            throw new PracticalNotExternalException();
        }

        var module = await practicalModulesRepository.GetByIdAsync(command.PracticalModuleId, cancellationToken);
        if (module is null || !module.IsEnabled)
        {
            logger.LogWarning(
                "Отклонено правило выборки практики {PracticalId}: модуль {ModuleId} не найден или отключён.",
                practicalId, command.PracticalModuleId);
            throw new PracticalModuleNotFoundException(command.PracticalModuleId);
        }

        var manualTasks = (await practicalsRepository.GetTasksAsync(practicalId, cancellationToken))
            .Where(task => task.PracticalModuleId is null)
            .ToList();
        if (manualTasks.Count > 0)
        {
            logger.LogWarning(
                "Отклонено правило выборки практики {PracticalId}: у практики есть ручной список заданий.",
                practicalId);
            throw new TaskSelectionModeConflictException(
                "У практики уже есть ручной список заданий — удалите его, прежде чем задавать правило выборки.");
        }

        // Режим случайной выборки: практика становится внешней, служебная связка с модулем снимается.
        await practicalsRepository.EnableRandomSelectionAsync(practicalId, cancellationToken);

        var rule = await rulesRepository.GetByPracticalIdAsync(practicalId, cancellationToken);
        if (rule is null)
        {
            rule = new TaskSelectionRule(
                practicalId, command.PracticalModuleId, command.TaskCount, command.DifficultyLevels, command.TopicRefs);
            await rulesRepository.AddAsync(rule, cancellationToken);
        }
        else
        {
            rule.Update(command.TaskCount, command.DifficultyLevels, command.TopicRefs);
        }

        await rulesRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Сохранено правило выборки практики {PracticalId}: модуль {ModuleId}, заданий {TaskCount}, сложности [{DifficultyLevels}], тем [{TopicRefs}].",
            practicalId, command.PracticalModuleId, rule.TaskCount,
            String.Join(',', rule.DifficultyLevels), String.Join(',', rule.TopicRefs));

        var emptySelectionWarning = !await HasCandidatesAsync(rule, cancellationToken);
        return new UpsertTaskSelectionRuleResult(ToInfo(rule), emptySelectionWarning);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteRuleAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);
        var rule = await rulesRepository.GetByPracticalIdAsync(practicalId, cancellationToken);
        if (rule is null)
        {
            return false;
        }

        if (await assignmentsRepository.HasAnySessionsForPracticalAsync(practicalId, cancellationToken))
        {
            logger.LogWarning(
                "Отклонено удаление правила выборки практики {PracticalId}: уже есть сессии модуля.", practicalId);
            throw new TaskSelectionRuleLockedException();
        }

        rulesRepository.Remove(rule);
        await rulesRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Правило выборки практики {PracticalId} удалено.", practicalId);
        return true;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StudentAssignmentRow>> ListAssignmentsAsync(
        Guid practicalId, Guid classWindowId, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);
        var assignments = await assignmentsRepository.ListAsync(practicalId, classWindowId, cancellationToken);
        var states = await LoadStatesAsync(assignments, cancellationToken);

        var binding = await practicalsRepository.GetAssignmentStartBindingAsync(practicalId, cancellationToken);
        var students = binding is null
            ? []
            : await classWindowsRepository.ListStudentsAsync(binding.CourseId, cancellationToken);

        return assignments
            .GroupBy(assignment => assignment.StudentId)
            .Select(group => new StudentAssignmentRow(
                group.Key,
                students.FirstOrDefault(student => student.UserId == group.Key)?.DisplayName
                    ?? group.Key.ToString(),
                group.OrderBy(assignment => assignment.SortOrder)
                    .Select(assignment => ToItem(assignment, states))
                    .ToList()))
            .OrderBy(row => row.StudentName, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    /// <inheritdoc />
    public async Task RegenerateAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);
        await taskAssignmentService.RegenerateForStudentAsync(practicalId, classWindowId, studentId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<MyAssignmentsInfo?> GetMyAssignmentsAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        var rule = await rulesRepository.GetByPracticalIdAsync(practicalId, cancellationToken);
        if (rule is null)
        {
            return null;
        }

        if (!await practicalsRepository.IsPracticalAssignedToStudentAsync(practicalId, userId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в чтении набора практики {PracticalId}: студент {UserId} не назначен на курс.",
                practicalId, userId);
            throw new StudentPracticalAccessDeniedException(practicalId);
        }

        var gate = await classWindowsRepository.GetGateForPracticalAsync(practicalId, cancellationToken);
        var window = gate is null
            ? null
            : await classAccess.FindOpenWindowAsync(gate.CourseId, userId, cancellationToken);
        if (window is not null)
        {
            // Ленивая догенерация: поздний вход, точечное добавление, сбой при открытии пары.
            await taskAssignmentService.EnsureForStudentAsync(practicalId, window.Id, userId, cancellationToken);
        }

        var windowId = window?.Id
            ?? await assignmentsRepository.FindLatestWindowIdForStudentAsync(practicalId, userId, cancellationToken);
        if (windowId is null)
        {
            return null;
        }

        var assignments = await assignmentsRepository.ListForStudentAsync(
            practicalId, windowId.Value, userId, cancellationToken);
        var states = await LoadStatesAsync(assignments, cancellationToken);
        return new MyAssignmentsInfo(
            windowId.Value,
            assignments.Select(assignment => ToItem(assignment, states)).ToList());
    }

    private async Task<bool> HasCandidatesAsync(TaskSelectionRule rule, CancellationToken cancellationToken)
    {
        try
        {
            var tasks = await moduleCatalogService.GetTasksAsync(rule.PracticalModuleId, cancellationToken) ?? [];
            return tasks.Any(task => TaskSelectionFilter.Matches(rule, task));
        }
        catch (ModuleCatalogUnavailableException exception)
        {
            logger.LogWarning(
                exception,
                "Не удалось проверить правило выборки практики {PracticalId} каталогом модуля {ModuleId}.",
                rule.PracticalMaterialId, rule.PracticalModuleId);
            return false;
        }
    }

    private async Task<IReadOnlyDictionary<Guid, AssignmentSessionInfo>> LoadStatesAsync(
        IReadOnlyList<PracticalTaskAssignment> assignments,
        CancellationToken cancellationToken)
    {
        var states = await sessionsRepository.ListForAssignmentsAsync(
            assignments.Select(assignment => assignment.Id).ToList(), cancellationToken);
        return states.ToDictionary(state => state.AssignmentId);
    }

    private static AssignmentTaskItem ToItem(
        PracticalTaskAssignment assignment,
        IReadOnlyDictionary<Guid, AssignmentSessionInfo> states) =>
        new(
            assignment.Id,
            assignment.ExternalTaskRef,
            assignment.SortOrder,
            states.GetValueOrDefault(assignment.Id)?.Status,
            states.GetValueOrDefault(assignment.Id)?.Grade);

    private static TaskSelectionRuleInfo ToInfo(TaskSelectionRule rule) => new(
        rule.PracticalMaterialId,
        rule.PracticalModuleId,
        rule.TaskCount,
        rule.DifficultyLevels,
        rule.TopicRefs);

    private async Task EnsurePracticalOwnerAsync(Guid practicalId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await practicalsRepository.IsPracticalOwnerAsync(practicalId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в доступе к правилу выборки практики {PracticalId}: пользователь {UserId} не владелец.",
                practicalId, legacyUserId);
            throw new CourseAccessDeniedException(practicalId);
        }
    }
}
