using System.Net;
using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.TestResults;
using Education.Tests.Auth;
using Education.Web.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Education.Tests.Practicals;

public sealed class PracticalsTestsApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public PracticalsTestsApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Student_StartsAssignedPracticalAttempt()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);

        var response = await client.PutAsync('/' + ApiRoutes.TestResults.ForStart(factory.Seed.AssignedStartPracticalId), null);

        response.EnsureSuccessStatusCode();
        var start = await response.Content.ReadFromJsonAsync<StartTestResponse>();
        Assert.Equal(1, start!.TryNumber);
    }

    [Fact]
    public async Task Student_CannotStartUnassignedPracticalAttempt()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);

        var response = await client.PutAsync('/' + ApiRoutes.TestResults.ForStart(factory.Seed.UnassignedPracticalId), null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SubmitAnswers_SavesCompletedProtocol()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);

        await client.PutAsync('/' + ApiRoutes.TestResults.ForStart(factory.Seed.SubmitPracticalId), null);
        var submitResponse = await client.PostAsJsonAsync(
            '/' + ApiRoutes.TestResults.ForSubmit(factory.Seed.SubmitPracticalId),
            new SubmitTestRequest([new SubmitAnswerRequest(factory.Seed.SubmitQuestionId, "a")]));

        submitResponse.EnsureSuccessStatusCode();
        var summary = await submitResponse.Content.ReadFromJsonAsync<TestProtocolSummaryResponse>();
        Assert.Equal(5, summary!.Grade);

        var protocol = await client.GetFromJsonAsync<TestProtocolResponse>('/' + ApiRoutes.TestResults.ForProtocol(summary.Id));
        var answer = Assert.Single(protocol!.Answers);
        Assert.True(answer.IsCorrect);
        Assert.Equal(1, answer.QuestionScore);
    }

    [Fact]
    public async Task TriesLimit_IsRespected()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);

        var response = await client.PutAsync('/' + ApiRoutes.TestResults.ForStart(factory.Seed.LimitedPracticalId), null);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Лимит попыток прохождения теста исчерпан.", problem!.Detail);
    }

    [Fact]
    public async Task Teacher_CannotReadOtherTeacherPracticalProtocols()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);

        var response = await client.GetAsync('/' + ApiRoutes.TestResults.ForTeacherPracticalProtocols(factory.Seed.OtherTeacherPracticalId));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
