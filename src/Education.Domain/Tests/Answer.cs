using Education.Domain.Common;

namespace Education.Domain.Tests;

public sealed class Answer : Entity
{
    public string Answers { get; private set; } = String.Empty;
    public Guid PracticalMaterialBindQuestionId { get; private set; }
    public PracticalMaterialBindQuestion PracticalMaterialBindQuestion { get; private set; } = null!;
    public Guid TestResultId { get; private set; }
    public TestResult TestResult { get; private set; } = null!;

    private Answer()
    {
    }

    public Answer(Guid practicalMaterialBindQuestionId, Guid testResultId, string answers)
    {
        PracticalMaterialBindQuestionId = practicalMaterialBindQuestionId;
        TestResultId = testResultId;
        Answers = answers;
    }
}

