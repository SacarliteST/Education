namespace Education.Application.Courses;

public sealed class CourseAccessDeniedException(Guid courseId)
    : InvalidOperationException($"Current user does not have access to course '{courseId}'.")
{
    public Guid CourseId { get; } = courseId;
}

