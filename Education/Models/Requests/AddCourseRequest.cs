using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class AddCourseRequest
{
    [Required]
    public DateTimeOffset Date { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public string Name { get; set; }
}
