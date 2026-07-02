using Education.Application.Common;
using Education.Domain.Courses;

namespace Education.Application.Courses;

/// <summary>
/// Предоставляет операции чтения и записи данных курсов для сценариев приложения.
/// </summary>
public interface ICoursesRepository : IBaseRepository<Course, long>
{
    /// <summary>
    /// Возвращает курсы указанного преподавателя.
    /// </summary>
    Task<IReadOnlyList<Course>> GetTeacherCoursesAsync(long teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт курс для указанного преподавателя.
    /// </summary>
    Task<Course> CreateCourseAsync(long teacherUserId, CreateCourseCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет курс по идентификатору.
    /// </summary>
    Task DeleteCourseAsync(long courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает курсы, назначенные указанному студенту.
    /// </summary>
    Task<IReadOnlyList<Course>> GetStudentCoursesAsync(long studentUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли курс указанному преподавателю.
    /// </summary>
    Task<bool> IsCourseOwnerAsync(long courseId, long teacherUserId, CancellationToken cancellationToken = default);
}
