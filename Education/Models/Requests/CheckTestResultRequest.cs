using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class CheckTestResultRequest
{
    [Required]
    public List<AnswerModel> Answers { get; set; }
    [Required]
    public long PracticalId { get; set; }
}

public class AnswerModel
{
    public long Id { get; set; }
    public string Answer { get; set; }
}
