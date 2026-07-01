using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class Course
{
    [Column("id")]
    public long Id { get; set; }
    [Column("c_name")]
    public string Name { get; set; }
    [Column("description")]
    public string Description { get; set; }
    [Column("date")]
    public DateTimeOffset Date { get; set; }
    [Column("user_id")]
    public long UserId { get; set; }
    public User User { get; set; }

    public List<Module> Modules { get; set; }
    public List<CourseBindUser> CourseBindUsers { get; set; }
}
