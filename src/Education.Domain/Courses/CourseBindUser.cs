using Education.Domain.Common;
using Education.Domain.Users;

namespace Education.Domain.Courses;

public sealed class CourseBindUser : Entity
{
    public long CourseId { get; private set; }
    public Course Course { get; private set; } = null!;
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;

    private CourseBindUser()
    {
    }

    public CourseBindUser(long courseId, long userId)
    {
        CourseId = courseId;
        UserId = userId;
    }
}
