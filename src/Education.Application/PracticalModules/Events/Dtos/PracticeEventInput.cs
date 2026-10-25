namespace Education.Application.PracticalModules;

/// <summary>Входные данные одного события «цифрового следа» (маппинг Kafka-сообщения в слое Web).</summary>
public sealed record PracticeEventInput(
    Guid SessionId,
    string SessionKey,
    Guid EventId,
    string Kind,
    DateTimeOffset OccurredAt,
    string PayloadJson,
    string? Markdown = null);
