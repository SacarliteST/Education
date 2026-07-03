namespace Education.Application.Courses;

/// <summary>
/// Команда создания курса.
/// </summary>
/// <param name="Date">Дата проведения курса.</param>
/// <param name="Description">Описание курса.</param>
/// <param name="Name">Название курса.</param>
public sealed record CreateCourseCommand(DateTimeOffset Date, string Description, string Name);

