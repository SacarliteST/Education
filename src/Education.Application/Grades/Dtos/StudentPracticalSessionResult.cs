using Education.Domain.PracticalModules;

namespace Education.Application.Grades;

/// <summary>Сессия прохождения задания внешнего модуля.</summary>
public sealed record StudentPracticalSessionResult(
    Guid SessionId,
    ModuleSessionState Status,
    ModuleSessionEndReason? EndReason,
    int? Grade,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt);
