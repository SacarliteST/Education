using Education.Domain.Practicals;
using Education.Domain.Tests;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Tests;

public class DomainRulesTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public DomainRulesTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Theory]
    [InlineData(90.0, 100.0, 5)]
    [InlineData(75.0, 100.0, 4)]
    [InlineData(60.0, 100.0, 3)]
    [InlineData(59.0, 100.0, 2)]
    [InlineData(null, 100.0, 0)]
    [InlineData(10.0, null, 0)]
    public async Task GradingPolicy_PreservesLegacyThresholds(double? score, double? maxScore, int expectedGrade)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var practical = await dbContext.PracticalMaterials.SingleAsync(item => item.Id == factory.Seed.SubmitPracticalId);

        var grade = practical.CalculateTestGrade(score, maxScore);

        Assert.Equal(expectedGrade, grade);
    }

    [Fact]
    public async Task PracticalMaterial_ChecksAttemptAvailability()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var practical = await dbContext.PracticalMaterials
            .Include(item => item.TestResults)
            .SingleAsync(item => item.Id == factory.Seed.LimitedPracticalId);

        Assert.False(practical.CanStartAttempt(practical.TestResults.Count(result => result.IsCompleted)));
    }

    [Fact]
    public async Task CaseFile_Accept_AssignsGradeAndLocksReplacement()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var file = await dbContext.CaseFiles.SingleAsync(item => item.Path == "other-student.txt");

        file.Accept(5);

        Assert.True(file.IsAccepted);
        Assert.Equal(5, file.Grade);
        Assert.Throws<InvalidOperationException>(() => file.ReplaceFile("Files/other.docx"));
    }

    [Fact]
    public async Task TestResult_Complete_StoresScoreAndPreventsSecondCompletion()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var result = await dbContext.TestResults.SingleAsync(item => item.PracticalMaterialId == factory.Seed.ProtocolPracticalId);
        var turnedAt = result.TurnedDate!.Value;

        Assert.True(result.IsCompleted);
        Assert.Equal(1, result.Score);
        Assert.Equal(1, result.MaxScore);
        Assert.Equal(turnedAt, result.TurnedDate);
        Assert.Throws<InvalidOperationException>(() => result.Complete(1, 1, turnedAt));
    }

    [Fact]
    public async Task QuestionScoringService_ScoresPersistedSingleChoiceQuestion()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var question = await dbContext.Questions
            .Where(item => item.Text == "Submit question")
            .SingleAsync();

        var score = QuestionScoringService.Score(question, "a");

        Assert.Equal(question.Weight, score.QuestionScore);
        Assert.True(score.IsCorrect);
    }

    [Fact]
    public async Task QuestionScoringService_ScoresPersistedWrongAnswerAsZero()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var question = await dbContext.Questions
            .Where(item => item.Text == "Submit question")
            .SingleAsync();

        var score = QuestionScoringService.Score(question, "b");

        Assert.Equal(0, score.QuestionScore);
        Assert.False(score.IsCorrect);
    }
}
