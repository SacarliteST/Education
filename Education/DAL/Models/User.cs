using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class User
{
    [Column("id")]
    public long Id { get; set; }
    [Column("login")]
    public string Login { get; set; }
    [Column("password")]
    public string Password { get; set; }
    [Column("first_name")]
    public string FirstName { get; set; }
    [Column("last_name")]
    public string LastName { get; set; }
    [Column("middle_name")]
    public string MiddleName { get; set; }
    [Column("role_id")]
    public long RoleId { get; set; }
    public Role Role { get; set; }
    public List<Course> Courses { get; set; }
    public List<CaseFile> CaseFiles { get; set; }
    public List<CourseBindUser> CourseBindUsers { get; set; }
    public List<PracticalBindUser> PracticalBindUsers { get; set; }
}
