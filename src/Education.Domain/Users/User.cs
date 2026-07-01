using Education.Domain.Common;
using Education.Domain.Courses;
using Education.Domain.Practicals;

namespace Education.Domain.Users;

public sealed class User : Entity
{
    public string Login { get; private set; } = String.Empty;
    public string Password { get; private set; } = String.Empty;
    public string FirstName { get; private set; } = String.Empty;
    public string LastName { get; private set; } = String.Empty;
    public string MiddleName { get; private set; } = String.Empty;
    public long RoleId { get; private set; }
    public Role Role { get; private set; } = null!;
    public List<Course> Courses { get; private set; } = [];
    public List<CaseFile> CaseFiles { get; private set; } = [];
    public List<CourseBindUser> CourseBindUsers { get; private set; } = [];
    public List<PracticalBindUser> PracticalBindUsers { get; private set; } = [];

    private User()
    {
    }

    public User(string login, string firstName, string lastName, string middleName, long roleId)
    {
        Login = login;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        RoleId = roleId;
    }
}
