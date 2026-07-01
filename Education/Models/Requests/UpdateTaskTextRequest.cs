using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class UpdateTaskTextRequest
{
    [Required]
    public long TaskId { get; set; }
    [Required]
    public string Text { get; set; }
}
