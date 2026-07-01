namespace Education.Models.QuestionAnswers;

public class MatchAnswerBase : AnswerBase
{
    public List<Match> Matches { get; set; } = new();
}

public class Match
{
    public string Left { get; set; }
    public string Right { get; set; }
    public bool IsCorrect { get; set; }
    public double Weight { get; set; }
}
