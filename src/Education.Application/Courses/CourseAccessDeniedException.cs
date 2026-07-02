namespace Education.Application.Courses;

public sealed class CourseAccessDeniedException(long courseId)
    : InvalidOperationException($"Current user does not have access to course '{courseId}'.")
{
    public long CourseId { get; } = courseId;
}
