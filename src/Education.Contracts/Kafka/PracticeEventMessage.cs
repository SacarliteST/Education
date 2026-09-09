using System.Text.Json;

namespace Education.Contracts.Kafka;

/// <summary>
/// Событие «цифрового следа» из топика <see cref="KafkaTopics.PracticeEvents"/>.
/// Что считать действием — решает модуль; ядро <see cref="Payload"/> не интерпретирует,
/// только складывает в ленту. Дедупликация — по <see cref="EventId"/>.
/// </summary>
public sealed record PracticeEventMessage(
    Guid SessionId,
    string SessionKey,
    Guid EventId,
    string Kind,
    DateTimeOffset OccurredAt,
    JsonElement Payload);
