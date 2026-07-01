using Education.Domain.Practicals;
using Education.Domain.Tests;

namespace Education.Tests;

public class DomainRulesTests
{
    [Theory]
    [InlineData(90.0, 100.0, 5)]
    [InlineData(75.0, 100.0, 4)]
    [InlineData(60.0, 100.0, 3)]
    [InlineData(59.0, 100.0, 2)]
    [InlineData(null, 100.0, 0)]
    [InlineData(10.0, null, 0)]
    public void GradingPolicy_PreservesLegacyThresholds(double? score, double? maxScore, int expectedGrade)
    {
        var grade = GradingPolicy.GetGrade(score, maxScore, 90, 75, 60);

        Assert.Equal(expectedGrade, grade);
    }

    [Fact]
    public void PracticalMaterial_ChecksAttemptAvailability()
    {
        var practical = new PracticalMaterial(10, "Practice");
        practical.ConfigureTest(2, 90, 75, 60);

        Assert.True(practical.CanStartAttempt(0));
        Assert.True(practical.CanStartAttempt(1));
        Assert.False(practical.CanStartAttempt(2));
    }

    [Fact]
    public void CaseFile_Accept_AssignsGradeAndLocksReplacement()
    {
        var file = new CaseFile(1, 42, "Files/work.docx");

        file.Accept(5);

        Assert.True(file.IsAccepted);
        Assert.Equal(5, file.Grade);
        Assert.Throws<InvalidOperationException>(() => file.ReplaceFile("Files/other.docx"));
    }

    [Fact]
    public void TestResult_Complete_StoresScoreAndPreventsSecondCompletion()
    {
        var result = new TestResult(42, 100, 1);
        var turnedAt = DateTime.UtcNow;

        result.Complete(8, 10, turnedAt);

        Assert.True(result.IsCompleted);
        Assert.Equal(8, result.Score);
        Assert.Equal(10, result.MaxScore);
        Assert.Equal(turnedAt, result.TurnedDate);
        Assert.Throws<InvalidOperationException>(() => result.Complete(9, 10, turnedAt));
    }
}
