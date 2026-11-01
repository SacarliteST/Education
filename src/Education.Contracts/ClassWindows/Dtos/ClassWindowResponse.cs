namespace Education.Contracts.ClassWindows;

/// <summary>Пара.</summary>
public sealed record ClassWindowResponse(
    Guid Id,
    Guid CourseId,
    string? GroupName,
    IReadOnlyList<ClassWindowStudentResponse> Students,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    DateTimeOffset? ClosedAt,
    bool IsActive);
