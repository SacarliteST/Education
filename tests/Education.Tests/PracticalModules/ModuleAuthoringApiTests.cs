using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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

    [Fact]
    public async Task OpenApi_DescribesAuthoringLink_CleanlyForOrval()
    {
        var openApi = await factory.CreateClient().GetStringAsync("/openapi/v1.json");
        using var document = JsonDocument.Parse(openApi);
        var root = document.RootElement;

        var operation = root
            .GetProperty("paths")
            .GetProperty("/api/v1/practical-modules/{practicalModuleId}/authoring-link")
            .GetProperty("post");
        Assert.Equal("CreatePracticalModuleAuthoringLink", operation.GetProperty("operationId").GetString());

        var schemaRef = operation
            .GetProperty("responses").GetProperty("200")
            .GetProperty("content").GetProperty("application/json")
            .GetProperty("schema").GetProperty("$ref").GetString();
        Assert.Equal("#/components/schemas/ModuleAuthoringLinkResponse", schemaRef);

        var expiresIn = root
            .GetProperty("components").GetProperty("schemas")
            .GetProperty("ModuleAuthoringLinkResponse")
            .GetProperty("properties").GetProperty("expiresInSeconds");
        // TD-001: числовое поле — чистый integer, без union [integer|string] и без pattern
        Assert.Equal(JsonValueKind.String, expiresIn.GetProperty("type").ValueKind);
        Assert.Equal("integer", expiresIn.GetProperty("type").GetString());
        Assert.False(expiresIn.TryGetProperty("pattern", out _));
    }

    [Fact]
    public async Task Teacher_RequestsAuthoringLink_WithReturnPathAndTask_PutsThemInQuery_TokenStaysInFragment()
    {
        var moduleId = await CreateModuleAsync();
        var exchange = new RecordingExchangeClient();

        var response = await TeacherClient(exchange).PostAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleAuthoringLink(moduleId),
            new CreateModuleAuthoringLinkRequest("/teacher/courses/c1/practicals/p1?tab=x", "task-42_A"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ModuleAuthoringLinkResponse>();
        Assert.EndsWith(
            "/teacher/launch?return=%2Fteacher%2Fcourses%2Fc1%2Fpracticals%2Fp1%3Ftab%3Dx&task=task-42_A" +
            "#access_token=" + RecordingExchangeClient.Token,
            body!.Url);
        Assert.Equal(1, body.Url.Count(character => character == '#'));
        Assert.DoesNotContain(RecordingExchangeClient.Token, body.Url.Split('#')[0]);
    }

    [Theory]
    [InlineData("//evil.example/x")]
    [InlineData("https://evil.example/x")]
    [InlineData("javascript:alert(1)")]
    [InlineData("teacher/courses")]
    [InlineData("/ok\\..\\evil")]
    [InlineData("/line\nbreak")]
    public async Task AuthoringLink_UnsafeReturnPath_IsRejected_WithoutTokenExchange(string returnPath)
    {
        var moduleId = await CreateModuleAsync();
        var exchange = new RecordingExchangeClient();

        var response = await TeacherClient(exchange).PostAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleAuthoringLink(moduleId),
            new CreateModuleAuthoringLinkRequest(returnPath, null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(exchange.Calls);
    }

    [Theory]
    [InlineData("a b")]
    [InlineData("../x")]
    [InlineData("x?y=1")]
    public async Task AuthoringLink_UnsafeTaskRef_IsRejected(string taskRef)
    {
        var moduleId = await CreateModuleAsync();

        var response = await TeacherClient(new RecordingExchangeClient()).PostAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ForModuleAuthoringLink(moduleId),
            new CreateModuleAuthoringLinkRequest(null, taskRef));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
