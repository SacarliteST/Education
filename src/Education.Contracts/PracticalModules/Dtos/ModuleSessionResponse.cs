using System.Text.Json;

namespace Education.Contracts.PracticalModules;

/// <summary>Статус попытки внешнего модуля.</summary>
public sealed record ModuleSessionResponse(
    Guid SessionId,
    string Status,
    int TryNumber,
    DateTimeOffset StartedAt,
    DateTimeOffset? ExpiresAt,
    string? EndReason,
    int? Grade,
    DateTimeOffset? EndedAt);
