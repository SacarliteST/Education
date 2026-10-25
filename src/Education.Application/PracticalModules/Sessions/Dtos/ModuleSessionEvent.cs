namespace Education.Application.PracticalModules;

/// <summary>Одно событие ленты попытки.</summary>
public sealed record ModuleSessionEvent(
    Guid EventId,
    string Kind,
    DateTimeOffset OccurredAt,
    string PayloadJson,
    string? Markdown = null);
