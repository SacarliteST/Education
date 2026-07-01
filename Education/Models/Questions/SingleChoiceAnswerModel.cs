using System.Runtime.InteropServices;

namespace Education.Models.Questions;

public class SingleChoiceAnswerModel
{
    public List<SingleChoiceOption> Answers { get; set; }
    public string CorrectAnswerId { get; set; }
}

public class SingleChoiceOption
{
    public string Id { get; set; }
    public string Text { get; set; }
}
