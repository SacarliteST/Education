namespace Education.Models.Questions;

public class MultipleAnswerModel
{
    public List<MultipleChoiceAnswerOptionModel> Answers { get; set; }
}

public class MultipleChoiceAnswerOptionModel
{
    public string Id { get; set; }
    public string Text { get; set; }
    public bool Correct { get; set; }
    public double Weight { get; set; }
}
