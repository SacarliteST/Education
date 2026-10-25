namespace Education.Contracts.Courses;

/// <summary>
/// Запрос на обновление набора студентов, назначенных на курс.
/// </summary>
/// <param name="UserIds">Полный желаемый набор идентификаторов студентов учебной системы.</param>
public sealed record UpdateCourseStudentsRequest(
    IReadOnlyList<Guid> UserIds);
