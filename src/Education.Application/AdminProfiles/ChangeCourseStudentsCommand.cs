namespace Education.Application.AdminProfiles;

/// <summary>
/// Команда точечного изменения набора студентов курса.
/// </summary>
/// <param name="CourseId">Идентификатор курса.</param>
/// <param name="Add">Студенты, которых нужно назначить.</param>
/// <param name="Remove">Студенты, с которых нужно снять назначение.</param>
public sealed record ChangeCourseStudentsCommand(Guid CourseId, IReadOnlyList<Guid> Add, IReadOnlyList<Guid> Remove);
