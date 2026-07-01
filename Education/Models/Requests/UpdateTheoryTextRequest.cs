using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class UpdateTheoryTextRequest
{
    [Required]
    public long TheoryId { get; set; }
    [Required]
    public string Text { get; set; }
}
