using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class AddTaskRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public long PracticalId { get; set; }
}
