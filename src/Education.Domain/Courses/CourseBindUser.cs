using Education.Domain.Common;
using Education.Domain.Users;

namespace Education.Domain.Courses;

/// <summary>
/// Связь курса с пользователем, которому курс доступен.
/// </summary>
public sealed class CourseBindUser : Entity
{
    /// <summary>
    /// Идентификатор курса.
    /// </summary>
    public Guid CourseId { get; private set; }

    /// <summary>
    /// Курс, доступный пользователю.
    /// </summary>
    public Course Course { get; private set; } = null!;

    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Пользователь, которому доступен курс.
    /// </summary>
    public User User { get; private set; } = null!;

    private CourseBindUser()
    {
    }

    /// <summary>
    /// Создает связь курса с пользователем.
    /// </summary>
    /// <param name="courseId">Идентификатор курса.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    public CourseBindUser(Guid courseId, Guid userId)
    {
        CourseId = courseId;
        UserId = userId;
    }
}

