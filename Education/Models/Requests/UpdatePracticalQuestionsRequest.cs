using System.ComponentModel.DataAnnotations;

namespace Education.Models.Requests;

public class UpdatePracticalQuestionsRequest
{
    [Required]
    public long PracticalId { get; set; }
    [Required]
    public List<long> QuestionIds { get; set; }
    [Required, Range(1, Int32.MaxValue)]
    public int TriesCount { get; set; }
    [Required, Range(1, Int32.MaxValue)]
    public double PercentForFive { get; set; }
    [Required, Range(1, Int32.MaxValue)]
    public double PercentForFour { get; set; }
    [Required, Range(1, Int32.MaxValue)]
    public double PercentForThree { get; set; }
}
