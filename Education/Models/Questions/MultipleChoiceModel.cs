namespace Education.Models.Questions;

public class MultipleChoiceModel
{
    public string Text { get; set; }
    public List<MultipleChoiceOption> Answers { get; set; }
}

public class MultipleChoiceOption
{
    public string Id { get; set; }
    public string Text { get; set; }
    public bool Correct { get; set; }
}
