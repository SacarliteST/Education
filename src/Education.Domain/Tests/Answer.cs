using Education.Domain.Common;

namespace Education.Domain.Tests;

public sealed class Answer : Entity
{
    public string Answers { get; private set; } = String.Empty;
    public long PracticalMaterialBindQuestionId { get; private set; }
    public PracticalMaterialBindQuestion PracticalMaterialBindQuestion { get; private set; } = null!;
    public long TestResultId { get; private set; }
    public TestResult TestResult { get; private set; } = null!;

    private Answer()
    {
    }

    public Answer(long practicalMaterialBindQuestionId, long testResultId, string answers)
    {
        PracticalMaterialBindQuestionId = practicalMaterialBindQuestionId;
        TestResultId = testResultId;
        Answers = answers;
    }
}
