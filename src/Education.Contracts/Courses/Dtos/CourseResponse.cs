namespace Education.Contracts.Courses;

/// <summary>
/// Данные курса, возвращаемые API системы обучения.
/// </summary>
public sealed record CourseResponse(
    Guid Id,
    DateTimeOffset Date,
    string Description,
    string Name);


