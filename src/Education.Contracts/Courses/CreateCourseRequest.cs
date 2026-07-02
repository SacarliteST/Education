namespace Education.Contracts.Courses;

/// <summary>
/// Запрос на создание курса, принадлежащего преподавателю.
/// </summary>
public sealed record CreateCourseRequest(
    DateTimeOffset Date,
    string Description,
    string Name);
