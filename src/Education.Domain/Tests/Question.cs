using Education.Domain.Common;
using Education.Domain.Courses;

namespace Education.Domain.Tests;

public sealed class Question : Entity
{
    public string Text { get; private set; } = String.Empty;
    public string Options { get; private set; } = String.Empty;
    public string Answer { get; private set; } = String.Empty;
    public double Weight { get; private set; }
    public Guid QuestionTypeId { get; private set; }
    public QuestionType QuestionType { get; private set; } = null!;
    public Guid ModuleId { get; private set; }
    public Module Module { get; private set; } = null!;
    public List<PracticalMaterialBindQuestion> PracticalMaterialBindQuestions { get; private set; } = [];

    private Question()
    {
    }

    public Question(Guid moduleId, Guid questionTypeId, string text, string options, string answer, double weight)
    {
        ModuleId = moduleId;
        QuestionTypeId = questionTypeId;
        Text = text;
        Options = options;
        Answer = answer;
        Weight = weight;
    }

    public void Update(Guid questionTypeId, string text, string options, string answer, double weight)
    {
        QuestionTypeId = questionTypeId;
        Text = text;
        Options = options;
        Answer = answer;
        Weight = weight;
    }
}

