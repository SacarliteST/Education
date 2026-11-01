namespace Education.Application.ClassWindows;

/// <summary>Пара в виде для API.</summary>
public sealed record ClassWindowInfo(
    Guid Id,
    Guid CourseId,
    string? GroupName,
    IReadOnlyList<ClassWindowStudentInfo> Students,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    DateTimeOffset? ClosedAt,
    bool IsActive);
