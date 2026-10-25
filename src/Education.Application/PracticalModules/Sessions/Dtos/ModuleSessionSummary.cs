using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

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
    DateTimeOffset? EndedAt,
    int? AttemptsCount = null);
