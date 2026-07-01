using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class AddTheoryRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public long ModuleId { get; set; }
}
