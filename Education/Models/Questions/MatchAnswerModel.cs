namespace Education.Models.Questions;

public class MatchAnswerModel
{
    public List<MatchOption> Matches { get; set; }
}

public class MatchOption
{
    public MatchHalf Left { get; set; }
    public MatchHalf Right { get; set; }
    public double Weight { get; set; }
}

public class MatchHalf
{
    public string Id { get; set; }
    public string Text { get; set; }
}

public class MatchUserAnswerModel
{
    public string Right { get; set; }
    public string Left { get; set; }
}
