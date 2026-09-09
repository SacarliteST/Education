namespace Education.Application.PracticalModules;

/// <summary>Жизненный цикл попыток внешнего практического модуля.</summary>
public interface IModuleSessionsService
{
    /// <summary>Запускает новую попытку или продолжает живую ACTIVE-сессию.</summary>
    /// <exception cref="ExternalTaskNotFoundException">Практика не внешняя / задание не привязано.</exception>
    /// <exception cref="TriesExhaustedException">Лимит запусков исчерпан.</exception>
    /// <exception cref="ModulePushFailedException">Модуль/IdentityService недоступны.</exception>
    Task<StartModuleSessionResult> StartAsync(Guid practicalId, Guid taskId, CancellationToken cancellationToken = default);

    /// <returns><see langword="null"/>, если сессия не найдена или принадлежит другому студенту.</returns>
    Task<ModuleSessionStatus?> GetStatusAsync(Guid practicalId, Guid sessionId, CancellationToken cancellationToken = default);

    /// <exception cref="ExternalTaskNotFoundException">Практика не внешняя / задание не привязано.</exception>
    Task<CurrentModuleSessionInfo> GetCurrentAsync(Guid practicalId, Guid taskId, CancellationToken cancellationToken = default);

    Task<AbandonOutcome> AbandonAsync(Guid practicalId, Guid sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Список всех попыток внешнего модуля по практике (для преподавателя — владельца курса).
    /// </summary>
    /// <returns><see langword="null"/>, если практики нет или текущий пользователь не преподаватель курса.</returns>
    Task<IReadOnlyList<ModuleSessionSummary>?> ListForPracticalAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Лента событий попытки. Доступ: владелец сессии (студент) или преподаватель курса практики.
    /// </summary>
    /// <returns><see langword="null"/>, если сессия не найдена, не относится к практике или доступа нет.</returns>
    Task<IReadOnlyList<ModuleSessionEvent>?> GetEventsAsync(
        Guid practicalId,
        Guid sessionId,
        CancellationToken cancellationToken = default);

    /// <summary>Приём оценки от модуля. Идемпотентно по факту <c>status=COMPLETED</c>.</summary>
    Task<CompleteOutcome> CompleteAsync(
        Guid sessionId,
        string sessionKey,
        int grade,
        string? completionData,
        CancellationToken cancellationToken = default);
}
