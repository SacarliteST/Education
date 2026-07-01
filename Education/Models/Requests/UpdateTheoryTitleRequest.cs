using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class UpdateTheoryTitleRequest
{
    [Required]
    public long TheoryId { get; set; }
    [Required]
    public string Title { get; set; }
}
