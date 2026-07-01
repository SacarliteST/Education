using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class UpdateCourseStudentsRequest
{
    [Required]
    public long CourseId { get; set; }
    [Required]
    public List<long> UserIds { get; set; }
}
