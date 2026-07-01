namespace Education.Models.QuestionAnswers;

public class MultipleAnswerBase : AnswerBase
{
    public List<Option> Options { get; set; } = new();
}

public class Option
{
    public string Id { get; set; }
    public string Text { get; set; }
    public double Weight { get; set; }
    public bool IsAnswer { get; set; }
    public bool IsCorrect { get; set; }
}
