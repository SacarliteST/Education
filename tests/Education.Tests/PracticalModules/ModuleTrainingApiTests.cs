using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Education.Application.PracticalModules;
using Education.Contracts;
using Education.Contracts.PracticalModules;
using Education.Tests.Auth;
using Education.Tests.Support;
using Education.Web.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Education.Tests.PracticalModules;

/// <summary>Вход студента в открытый банк заданий: список модулей и ссылка с токеном во фрагменте.</summary>
public sealed class ModuleTrainingApiTests : IClassFixture<TestWebApplicationFactory>
{
    private const string BankOn = "{\"trainingBank\":{\"enabled\":true}}";
    private readonly TestWebApplicationFactory factory;

    public ModuleTrainingApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Student_GetsLink_TokenOnlyInFragment_WithoutSession()
    {
        var moduleId = await factory.RegisterModuleAsync(BankOn);
        var exchange = new RecordingTokenExchangeClient();

        var response = await StudentClient(exchange).PostAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleTrainingLink(moduleId),
            new CreateTrainingLinkRequest("/student/training"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var link = (await response.Content.ReadFromJsonAsync<ModuleTrainingLinkResponse>())!;
        var url = new Uri(link.Url);
        Assert.EndsWith("/student/launch", url.AbsolutePath);
        Assert.Equal("?return=%2Fstudent%2Ftraining", url.Query);
        Assert.Equal("#access_token=" + RecordingTokenExchangeClient.Token, url.Fragment);
        Assert.DoesNotContain(RecordingTokenExchangeClient.Token, link.Url.Split('#')[0]);
        Assert.DoesNotContain("session", url.Query);
        var call = Assert.Single(exchange.Calls);
        Assert.Equal("sql-module-api", call.Audience);
        Assert.Null(call.SessionId);
    }

    [Fact]
    public async Task Link_UsesLaunchPath_FromConfiguration()
    {
        var moduleId = await factory.RegisterModuleAsync("{\"trainingBank\":{\"enabled\":true,\"launchPath\":\"/student/bank\"}}");

        var response = await StudentClient(new RecordingTokenExchangeClient()).PostAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleTrainingLink(moduleId), null);

        var link = (await response.Content.ReadFromJsonAsync<ModuleTrainingLinkResponse>())!;
        Assert.EndsWith("/student/bank", new Uri(link.Url).AbsolutePath);
        Assert.Equal(String.Empty, new Uri(link.Url).Query);
    }

    [Theory]
    [InlineData(EducationRoles.Teacher)]
    [InlineData(EducationRoles.Admin)]
    public async Task NonStudent_Gets403(string role)
    {
        var moduleId = await factory.RegisterModuleAsync(BankOn);
        var client = ClientWith(new RecordingTokenExchangeClient(), role);

        var response = await client.PostAsync('/' + ApiRoutes.PracticalModules.ForModuleTrainingLink(moduleId), null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UnknownModule_Returns404()
    {
        var response = await StudentClient(new RecordingTokenExchangeClient()).PostAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleTrainingLink(Guid.NewGuid()), null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DisabledModule_Returns409_ModuleDisabled()
    {
        var moduleId = await factory.RegisterModuleAsync(BankOn, enabled: false);

        var response = await StudentClient(new RecordingTokenExchangeClient()).PostAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleTrainingLink(moduleId), null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("ModuleDisabled", await ReadReasonAsync(response));
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"trainingBank\":{\"enabled\":false}}")]
    [InlineData("{\"trainingBank\":{\"enabled\":true,\"launchPath\":\"//evil.example\"}}")]
    public async Task BankNotEnabled_Returns409_TrainingBankDisabled(string configuration)
    {
        var moduleId = await factory.RegisterModuleAsync(configuration);
        var exchange = new RecordingTokenExchangeClient();

        var response = await StudentClient(exchange).PostAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleTrainingLink(moduleId), null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("TrainingBankDisabled", await ReadReasonAsync(response));
        Assert.Empty(exchange.Calls);
    }

    [Theory]
    [InlineData("https://evil.example/x")]
    [InlineData("//evil.example")]
    [InlineData("/ok\\bad")]
    public async Task InvalidReturnPath_Returns400(string returnPath)
    {
        var moduleId = await factory.RegisterModuleAsync(BankOn);

        var response = await StudentClient(new RecordingTokenExchangeClient()).PostAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleTrainingLink(moduleId),
            new CreateTrainingLinkRequest(returnPath));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task IdentityDown_Returns502()
    {
        var moduleId = await factory.RegisterModuleAsync(BankOn);

        var response = await StudentClient(new RecordingTokenExchangeClient { Throw = true }).PostAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleTrainingLink(moduleId), null);

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
        Assert.Equal("IdentityUnavailable", await ReadReasonAsync(response));
    }

    [Fact]
    public async Task TrainingList_ReturnsOnlyEnabledModulesWithBank()
    {
        var withBank = await factory.RegisterModuleAsync(BankOn);
        var withoutBank = await factory.RegisterModuleAsync("{}");
        var disabled = await factory.RegisterModuleAsync(BankOn, enabled: false);

        var modules = await StudentClient(new RecordingTokenExchangeClient())
            .GetFromJsonAsync<List<TrainingModuleResponse>>('/' + ApiRoutes.PracticalModules.TrainingModulesList);

        Assert.Contains(modules!, module => module.Id == withBank);
        Assert.DoesNotContain(modules!, module => module.Id == withoutBank);
        Assert.DoesNotContain(modules!, module => module.Id == disabled);
    }

    [Fact]
    public async Task TrainingList_IsClosedForTeacher()
    {
        var client = ClientWith(new RecordingTokenExchangeClient(), EducationRoles.Teacher);

        var response = await client.GetAsync('/' + ApiRoutes.PracticalModules.TrainingModulesList);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private HttpClient StudentClient(RecordingTokenExchangeClient exchange) => ClientWith(exchange, EducationRoles.Student);

    private HttpClient ClientWith(RecordingTokenExchangeClient exchange, string role)
    {
        var client = factory
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<ITokenExchangeClient>();
                services.AddSingleton<ITokenExchangeClient>(exchange);
            }))
            .CreateClient();
        client.AuthenticateAs(role);
        return client;
    }

    private static async Task<string?> ReadReasonAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.TryGetProperty("reason", out var reason) ? reason.GetString() : null;
    }
}
