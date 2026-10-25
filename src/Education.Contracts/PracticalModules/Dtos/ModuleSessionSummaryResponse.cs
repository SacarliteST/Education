using System.Text.Json;

namespace Education.Contracts.PracticalModules;

/// <summary>Строка списка попыток внешнего модуля для преподавателя.</summary>
public sealed record ModuleSessionSummaryResponse(
    Guid SessionId,
    Guid UserId,
    string StudentName,
    int TryNumber,
    string Status,
    string? EndReason,
    int? Grade,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt,
    int? AttemptsCount = null);
