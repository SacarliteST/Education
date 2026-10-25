using System.Text.Json;

namespace Education.Contracts.PracticalModules;

/// <summary>Одно событие ленты «цифрового следа» попытки.</summary>
public sealed record ModuleSessionEventResponse(
    Guid EventId,
    string Kind,
    DateTimeOffset OccurredAt,
    JsonElement Payload,
    string? Markdown = null);
