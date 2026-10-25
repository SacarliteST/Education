using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

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
