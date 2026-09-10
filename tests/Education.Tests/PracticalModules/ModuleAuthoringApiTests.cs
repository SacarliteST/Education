using System.Net;
using System.Net.Http.Json;
using Education.Application.PracticalModules;
using Education.Contracts;
using Education.Contracts.PracticalModules;
using Education.Tests.Auth;
using Education.Web.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Education.Tests.PracticalModules;

public sealed class ModuleAuthoringApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public ModuleAuthoringApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Teacher_RequestsAuthoringLink_ReturnsModuleTeacherLaunchUrl()
    {
        var moduleId = await CreateModuleAsync();
        var exchange = new RecordingExchangeClient();

        var response = await TeacherClient(exchange).PostAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleAuthoringLink(moduleId), null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ModuleAuthoringLinkResponse>();
        Assert.EndsWith("/teacher/launch#access_token=" + RecordingExchangeClient.Token, body!.Url);
        Assert.DoesNotContain("session=", body.Url);
        Assert.True(body.ExpiresInSeconds > 0);

        // обмен идёт под audience модуля и без сессии
        var call = Assert.Single(exchange.Calls);
        Assert.Equal("sql-module-api", call.Audience);
        Assert.Null(call.SessionId);
    }

    [Fact]
    public async Task Student_RequestsAuthoringLink_Forbidden()
    {
        var moduleId = await CreateModuleAsync();
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);

        var response = await client.PostAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleAuthoringLink(moduleId), null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task AuthoringLink_UnknownModule_NotFound()
    {
        var response = await TeacherClient(new RecordingExchangeClient()).PostAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleAuthoringLink(Guid.NewGuid()), null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AuthoringLink_DisabledModule_Conflict()
    {
        var moduleId = await CreateModuleAsync(enabled: false);

        var response = await TeacherClient(new RecordingExchangeClient()).PostAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleAuthoringLink(moduleId), null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AuthoringLink_IdentityUnavailable_BadGateway()
    {
        var moduleId = await CreateModuleAsync();

        var response = await TeacherClient(new RecordingExchangeClient { Throw = true }).PostAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleAuthoringLink(moduleId), null);

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
    }

    // ---- helpers ----

    private async Task<Guid> CreateModuleAsync(bool enabled = true)
    {
        var admin = factory.CreateClient();
        admin.AuthenticateAs(EducationRoles.Admin);

        var slug = "sql-" + Guid.NewGuid().ToString("N")[..8];
        var created = await admin.PostAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ModulesList,
            new CreatePracticalModuleRequest(
                slug, "SQL", "d", "SQL_SIMULATOR", "/modules/sql", "sql-module-api", "{}"));
        created.EnsureSuccessStatusCode();
        var moduleId = (await created.Content.ReadFromJsonAsync<PracticalModuleResponse>())!.Id;

        if (!enabled)
        {
            var disabled = await admin.PutAsJsonAsync(
                '/' + ApiRoutes.PracticalModules.ForModule(moduleId),
                new UpdatePracticalModuleRequest(
                    "SQL", "d", "SQL_SIMULATOR", "/modules/sql", "sql-module-api", "{}", IsEnabled: false));
            disabled.EnsureSuccessStatusCode();
        }

        return moduleId;
    }

    private HttpClient TeacherClient(ITokenExchangeClient exchange)
    {
        var client = factory
            .WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<ITokenExchangeClient>();
                services.AddSingleton(exchange);
            }))
            .CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);
        return client;
    }

    private sealed class RecordingExchangeClient : ITokenExchangeClient
    {
        public const string Token = "authoring.jwt.value";

        public bool Throw { get; init; }

        public List<(string Audience, Guid? SessionId)> Calls { get; } = [];

        public Task<ExchangedToken> ExchangeAsync(
            string audience, Guid? sessionId = null, DateTimeOffset? sessionExpiresAt = null,
            CancellationToken cancellationToken = default)
        {
            Calls.Add((audience, sessionId));
            if (Throw)
            {
                throw new ModulePushFailedException("test: identity down");
            }

            return Task.FromResult(new ExchangedToken(Token, 900));
        }
    }
}
