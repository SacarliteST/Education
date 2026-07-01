using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class AddPracticalRequest
{
    [Required]
    public long ModuleId { get; set; }
    [Required]
    public string Name { get; set; }
}
