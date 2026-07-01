using System.ComponentModel.DataAnnotations;
using Education.Consts;

namespace Education.Models.Requests;

public class AddQuestionRequest
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
    public long ModuleId { get; set; }
    [Required]
    public QuestionTypeEnum Type { get; set; }
}
