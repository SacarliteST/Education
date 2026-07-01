using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class AddModuleRequest
{
    [Required]
    public long CourseId { get; set; }
    [Required]
    public string Name { get; set; }
}
