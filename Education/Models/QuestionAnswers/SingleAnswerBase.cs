namespace Education.Models.QuestionAnswers;

public class SingleAnswerBase : AnswerBase
{
    public List<SingleOption> Answers { get; set; } = new();
}

public class SingleOption
{
    public string Id { get; set; }
    public string Text { get; set; }
    public bool IsAnswer { get; set; }
    public bool IsCorrect { get; set; }
}
