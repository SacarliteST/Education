namespace Education.Contracts.Courses;

/// <summary>
/// Данные курса, возвращаемые API системы обучения.
/// </summary>
public sealed record CourseResponse(
    long Id,
    DateTimeOffset Date,
    string Description,
    string Name);
