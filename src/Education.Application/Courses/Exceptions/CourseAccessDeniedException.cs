namespace Education.Application.Courses;

/// <summary>Текущий пользователь не имеет доступа к курсу.</summary>
/// <param name="courseId">Идентификатор курса.</param>
public sealed class CourseAccessDeniedException(Guid courseId)
    : InvalidOperationException($"Current user does not have access to course '{courseId}'.")
{
    /// <summary>Идентификатор курса, к которому отказано в доступе.</summary>
    public Guid CourseId { get; } = courseId;
}

