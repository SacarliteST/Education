namespace Education.Application.AdminProfiles;

/// <summary>
/// Команда обновления набора студентов, назначенных на курс.
/// </summary>
/// <param name="CourseId">Идентификатор курса.</param>
/// <param name="UserIds">Полный желаемый набор идентификаторов студентов учебной системы.</param>
public sealed record SetCourseStudentsCommand(Guid CourseId, IReadOnlyList<Guid> UserIds);
