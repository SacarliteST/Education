using Education.Domain.Common;
using Education.Domain.Users;

namespace Education.Domain.Courses;

public sealed class Course : Entity
{
    public string Name { get; private set; } = String.Empty;
    public string Description { get; private set; } = String.Empty;
    public DateTimeOffset Date { get; private set; }
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public List<Module> Modules { get; private set; } = [];
    public List<CourseBindUser> CourseBindUsers { get; private set; } = [];

    private Course()
    {
    }

    public Course(string name, string description, DateTimeOffset date, long userId)
    {
        Name = name;
        Description = description;
        Date = date;
        UserId = userId;
    }
}
