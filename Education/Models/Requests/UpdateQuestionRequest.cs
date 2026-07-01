using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class UpdateQuestionRequest
{
    [Required]
    public string Text { get; set; }
    [Required]
    public string Body { get; set; }
    [Required]
    public string Answer { get; set; }
    [Required]
    public double Weight { get; set; }
    [Required]
    public long QuestionId { get; set; }
}
