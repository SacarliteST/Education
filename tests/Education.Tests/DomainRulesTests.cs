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
        var practical = new PracticalMaterial(Guid.NewGuid(), "Practical");

        var grade = practical.CalculateTestGrade(score, maxScore);

        Assert.Equal(expectedGrade, grade);
    }

    [Fact]
    public void PracticalMaterial_ChecksAttemptAvailability()
    {
        var practical = new PracticalMaterial(Guid.NewGuid(), "Practical");
        var result = new TestResult(Guid.NewGuid(), practical.Id, 1);
        result.Complete(1, 1, DateTime.UtcNow);
        practical.TestResults.Add(result);

        Assert.False(practical.CanStartAttempt(practical.TestResults.Count(result => result.IsCompleted)));
    }

    [Fact]
    public void CaseFile_Accept_AssignsGradeAndLocksReplacement()
    {
        var file = new CaseFile(Guid.NewGuid(), Guid.NewGuid(), "solution.docx");

        file.Accept(5);

        Assert.True(file.IsAccepted);
        Assert.Equal(5, file.Grade);
        Assert.Throws<InvalidOperationException>(() => file.ReplaceFile("Files/other.docx"));
    }

    [Fact]
    public void TestResult_Complete_StoresScoreAndPreventsSecondCompletion()
    {
        var turnedAt = DateTime.UtcNow;
        var result = new TestResult(Guid.NewGuid(), Guid.NewGuid(), 1);

        result.Complete(1, 1, turnedAt);

        Assert.True(result.IsCompleted);
        Assert.Equal(1, result.Score);
        Assert.Equal(1, result.MaxScore);
        Assert.Equal(turnedAt, result.TurnedDate);
        Assert.Throws<InvalidOperationException>(() => result.Complete(1, 1, turnedAt));
    }

    [Fact]
    public void QuestionScoringService_ScoresSingleChoiceQuestion()
    {
        var question = CreateSingleChoiceQuestion();

        var score = QuestionScoringService.Score(question, "a");

        Assert.Equal(question.Weight, score.QuestionScore);
        Assert.True(score.IsCorrect);
    }

    [Fact]
    public void QuestionScoringService_ScoresWrongAnswerAsZero()
    {
        var question = CreateSingleChoiceQuestion();

        var score = QuestionScoringService.Score(question, "b");

        Assert.Equal(0, score.QuestionScore);
        Assert.False(score.IsCorrect);
    }

    private static Question CreateSingleChoiceQuestion()
    {
        const string body = """
            {"answers":[{"id":"a","text":"Right"},{"id":"b","text":"Wrong"}]}
            """;
        const string answer = """
            {"answers":[{"id":"a","text":"Right"},{"id":"b","text":"Wrong"}],"correctAnswerId":"a"}
            """;

        return new Question(Guid.NewGuid(), QuestionTypeIds.SingleChoice, "Question", body, answer, 1);
    }
}
