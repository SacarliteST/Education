using Education.Domain.Common;

namespace Education.Domain.PracticalModules;

/// <summary>
/// Запись «цифрового следа» — одно действие студента в модуле. Что считать действием,
/// решает модуль; ядро <see cref="Payload"/> не интерпретирует. Дедупликация — по <see cref="Entity.Id"/>
/// (это <c>eventId</c> из Kafka-сообщения).
/// </summary>
public sealed class PracticalTaskEvent : Entity
{
    /// <summary>Сессия, к которой относится событие.</summary>
    public Guid SessionId { get; private set; }

    /// <summary>Произвольная строка-тип от модуля («sql_submit» и т.п.).</summary>
    public string Kind { get; private set; } = String.Empty;

    /// <summary>Момент действия по данным модуля.</summary>
    public DateTimeOffset OccurredAt { get; private set; }

    /// <summary>Произвольный JSON от модуля (jsonb).</summary>
    public string Payload { get; private set; } = "{}";

    private PracticalTaskEvent()
    {
    }

    public PracticalTaskEvent(Guid eventId, Guid sessionId, string kind, DateTimeOffset occurredAt, string payload)
    {
        Id = eventId;
        SessionId = sessionId;
        Kind = kind;
        OccurredAt = occurredAt;
        Payload = String.IsNullOrWhiteSpace(payload) ? "{}" : payload;
    }
}
