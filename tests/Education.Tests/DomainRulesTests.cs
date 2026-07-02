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

    [Theory]
    [InlineData(QuestionKind.SingleChoice, """{"answers":[{"id":"a","text":"Right"}],"correctAnswerId":"a"}""", "a", 1)]
    [InlineData(QuestionKind.ShortAnswer, """{"answer":"Alpha;Beta"}""", "beta", 1)]
    [InlineData(QuestionKind.ShortAnswer, """{"answer":"Alpha;Beta"}""", "gamma", 0)]
    public void QuestionScoringService_ScoresSimpleQuestionTypes(
        QuestionKind kind,
        string answer,
        string userAnswer,
        double expectedScore)
    {
        var question = new Question(1, (long)kind, "Question", "{}", answer, 2);

        var score = QuestionScoringService.Score(question, userAnswer);

        Assert.Equal(expectedScore * 2, score.QuestionScore);
    }

    [Fact]
    public void QuestionScoringService_ScoresMultipleChoice()
    {
        const string answer = """
            {"answers":[{"id":"a","text":"A","correct":true,"weight":0.5},{"id":"b","text":"B","correct":false,"weight":0.5}]}
            """;
        var question = new Question(1, (long)QuestionKind.MultipleChoice, "Question", "{}", answer, 4);

        var score = QuestionScoringService.Score(question, """["a"]""");

        Assert.Equal(4, score.QuestionScore);
    }

    [Fact]
    public void QuestionScoringService_ScoresMatch()
    {
        const string answer = """
            {"matches":[{"left":{"id":"l","text":"Left"},"right":{"id":"r","text":"Right"},"weight":1}]}
            """;
        var question = new Question(1, (long)QuestionKind.Match, "Question", "{}", answer, 3);

        var score = QuestionScoringService.Score(question, """[{"left":"l","right":"r"}]""");

        Assert.Equal(3, score.QuestionScore);
    }
}
