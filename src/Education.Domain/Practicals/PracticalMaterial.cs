using Education.Domain.Common;
using Education.Domain.Courses;
using Education.Domain.Tests;

namespace Education.Domain.Practicals;

public sealed class PracticalMaterial : Entity
{
    public string Name { get; private set; } = String.Empty;
    public Guid ModuleId { get; private set; }
    public bool IsPublic { get; private set; }
    public int TriesCount { get; private set; } = 1;
    public double PercentForFive { get; private set; } = 90;
    public double PercentForFour { get; private set; } = 75;
    public double PercentForThree { get; private set; } = 60;
    public Module Module { get; private set; } = null!;
    public List<Case> Cases { get; private set; } = [];
    public List<PracticalMaterialBindQuestion> PracticalMaterialBindQuestions { get; private set; } = [];
    public List<TestResult> TestResults { get; private set; } = [];
    public List<PracticalBindUser> PracticalBindUsers { get; private set; } = [];

    private PracticalMaterial()
    {
    }

    public PracticalMaterial(Guid moduleId, string name)
    {
        ModuleId = moduleId;
        Name = name;
    }

    public void Publish()
    {
        IsPublic = true;
    }

    public bool CanStartAttempt(int completedAttemptCount) => completedAttemptCount < TriesCount;

    public void ConfigureTest(int triesCount, double percentForFive, double percentForFour, double percentForThree)
    {
        if (triesCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(triesCount));
        }

        TriesCount = triesCount;
        PercentForFive = percentForFive;
        PercentForFour = percentForFour;
        PercentForThree = percentForThree;
    }

    public int CalculateTestGrade(double? score, double? maxScore)
    {
        return GradingPolicy.GetGrade(score, maxScore, PercentForFive, PercentForFour, PercentForThree);
    }

    public int CalculateFinalGrade(int bestTestGrade, double meanTaskGrade)
    {
        return (int)Math.Ceiling((bestTestGrade + meanTaskGrade) / 2);
    }
}

