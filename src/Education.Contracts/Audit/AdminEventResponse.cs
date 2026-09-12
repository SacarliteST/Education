namespace Education.Contracts.Audit;

/// <summary>Запись журнала административных действий Education.</summary>
public sealed record AdminEventResponse(
    Guid Id,
    Guid? ActorIdentityUserId,
    string? ActorName,
    string EventType,
    string Description,
    DateTimeOffset CreatedAt);
