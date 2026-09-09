using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

/// <summary>Результат запуска/продолжения попытки внешнего модуля.</summary>
public sealed record StartModuleSessionResult(
    Guid SessionId,
    string LaunchUrl,
    DateTimeOffset? ExpiresAt,
    int TryNumber,
    bool Resumed);

/// <summary>Статус конкретной попытки.</summary>
public sealed record ModuleSessionStatus(
    Guid SessionId,
    ModuleSessionState State,
    int TryNumber,
    DateTimeOffset StartedAt,
    DateTimeOffset? ExpiresAt,
    ModuleSessionEndReason? EndReason,
    int? Grade,
    DateTimeOffset? EndedAt);

/// <summary>Данные для гейта кнопок на странице практики.</summary>
public sealed record CurrentModuleSessionInfo(
    ModuleSessionStatus? Session,
    int AttemptsCount,
    int TriesCount,
    int? TimeLimitMinutes,
    int? BestGrade);

/// <summary>Одно событие ленты попытки.</summary>
public sealed record ModuleSessionEvent(
    Guid EventId,
    string Kind,
    DateTimeOffset OccurredAt,
    string PayloadJson);

/// <summary>Строка списка попыток внешнего модуля для преподавателя.</summary>
public sealed record ModuleSessionSummary(
    Guid SessionId,
    Guid UserId,
    string StudentName,
    int TryNumber,
    ModuleSessionState State,
    ModuleSessionEndReason? EndReason,
    int? Grade,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt);

/// <summary>Итог попытки прервать сессию.</summary>
public enum AbandonOutcome
{
    NotFound,
    AlreadyTerminal,
    Abandoned,
}

/// <summary>Итог приёма оценки от модуля.</summary>
public enum CompleteOutcome
{
    Unauthorized,
    Accepted,
    AlreadyCompleted,
    SessionClosed,
}
