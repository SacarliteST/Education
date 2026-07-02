using Education.Domain.Courses;

namespace Education.Application.Courses;

/// <summary>
/// Выполняет сценарии работы с курсами.
/// </summary>
public interface ICoursesService
{
    /// <summary>
    /// Возвращает курсы, принадлежащие текущему преподавателю.
    /// </summary>
    Task<IReadOnlyList<Course>> GetTeacherCoursesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт курс для текущего преподавателя.
    /// </summary>
    Task<Course> CreateCourseAsync(CreateCourseCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет курс текущего преподавателя.
    /// </summary>
    Task DeleteCourseAsync(long courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает курсы, назначенные текущему студенту.
    /// </summary>
    Task<IReadOnlyList<Course>> GetStudentCoursesAsync(CancellationToken cancellationToken = default);
}
