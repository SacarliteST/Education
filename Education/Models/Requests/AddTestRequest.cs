using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class AddTestRequest
{
    [Required]
    public long PracticalId { get; set; }
    [Required]
    public long[] QuestionIds { get; set; }
}
