using Education.Domain.Common;
using Education.Domain.Users;

namespace Education.Domain.Courses;

public sealed class CourseBindUser : Entity
{
    public Guid CourseId { get; private set; }
    public Course Course { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    private CourseBindUser()
    {
    }

    public CourseBindUser(Guid courseId, Guid userId)
    {
        CourseId = courseId;
        UserId = userId;
    }
}

