namespace Education.Domain.Audit;

/// <summary>
/// Запись журнала административных действий Education — то, что не видно
/// в аудите IdentityService (регистрация модулей, привязка профилей,
/// изменения курсов/практик). См. SQLTren/PLATFORM.md, бэклог
/// логирования кода, задача 3.
/// </summary>
public sealed class AdminEvent
{
    public Guid Id { get; private set; }

    /// <summary>Identity User Id инициатора действия; null, если действие системное.</summary>
    public Guid? ActorIdentityUserId { get; private set; }

    /// <summary>Отображаемое имя инициатора на момент события.</summary>
    public string? ActorName { get; private set; }

    public string EventType { get; private set; } = String.Empty;

    public string Description { get; private set; } = String.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    private AdminEvent() { }

    public static AdminEvent Create(
        Guid? actorIdentityUserId,
        string? actorName,
        string eventType,
        string description,
        DateTimeOffset createdAt) =>
        new()
        {
            Id = Guid.NewGuid(),
            ActorIdentityUserId = actorIdentityUserId,
            ActorName = actorName,
            EventType = eventType,
            Description = description,
            CreatedAt = createdAt,
        };
}
