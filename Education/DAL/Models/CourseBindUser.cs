using System.ComponentModel.DataAnnotations.Schema;

namespace Education.DAL.Models;

public class CourseBindUser
{
    [Column("id")]
    public long Id { get; set; }
    [Column("course_id")]
    public long CourseId { get; set; }
    public Course Course { get; set; }
    [Column("user_id")]
    public long UserId { get; set; }
    public User User { get; set; }
}
