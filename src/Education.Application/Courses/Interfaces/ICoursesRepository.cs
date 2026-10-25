using Education.Application.Common;
using Education.Domain.Courses;

namespace Education.Application.Courses;

/// <summary>
/// Предоставляет операции чтения и записи данных курсов для сценариев приложения.
/// </summary>
public interface ICoursesRepository : IBaseRepository<Course, Guid>
{
    /// <summary>
    /// Возвращает курсы указанного преподавателя.
    /// </summary>
    Task<IReadOnlyList<Course>> GetTeacherCoursesAsync(Guid teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт курс для указанного преподавателя.
    /// </summary>
    Task<Course> CreateCourseAsync(Guid teacherUserId, CreateCourseCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет курс по идентификатору.
    /// </summary>
    Task DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает курсы, назначенные указанному студенту.
    /// </summary>
    Task<IReadOnlyList<Course>> GetStudentCoursesAsync(Guid studentUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли курс указанному преподавателю.
    /// </summary>
    Task<bool> IsCourseOwnerAsync(Guid courseId, Guid teacherUserId, CancellationToken cancellationToken = default);
}

