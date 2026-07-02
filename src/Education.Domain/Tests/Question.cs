using Education.Domain.Common;
using Education.Domain.Courses;

namespace Education.Domain.Tests;

public sealed class Question : Entity
{
    public string Text { get; private set; } = String.Empty;
    public string Options { get; private set; } = String.Empty;
    public string Answer { get; private set; } = String.Empty;
    public double Weight { get; private set; }
    public long QuestionTypeId { get; private set; }
    public QuestionType QuestionType { get; private set; } = null!;
    public long ModuleId { get; private set; }
    public Module Module { get; private set; } = null!;
    public List<PracticalMaterialBindQuestion> PracticalMaterialBindQuestions { get; private set; } = [];

    private Question()
    {
    }

    public Question(long moduleId, long questionTypeId, string text, string options, string answer, double weight)
    {
        ModuleId = moduleId;
        QuestionTypeId = questionTypeId;
        Text = text;
        Options = options;
        Answer = answer;
        Weight = weight;
    }

    public void Update(long questionTypeId, string text, string options, string answer, double weight)
    {
        QuestionTypeId = questionTypeId;
        Text = text;
        Options = options;
        Answer = answer;
        Weight = weight;
    }
}
