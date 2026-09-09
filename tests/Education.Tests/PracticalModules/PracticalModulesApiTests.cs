using System.Net;
using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.PracticalModules;
using Education.Tests.Auth;
using Education.Web.Identity;

namespace Education.Tests.PracticalModules;

public sealed class PracticalModulesApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public PracticalModulesApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Admin_CreatesModule_ThenListedAndFetchable()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Admin);
        var slug = "sql-" + Guid.NewGuid().ToString("N")[..8];

        var createResponse = await client.PostAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ModulesList,
            new CreatePracticalModuleRequest(
                slug, "SQL-тренажёр", "Практика по SQL", "SQL_SIMULATOR", "/modules/sql", "sql-module-api", null));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<PracticalModuleResponse>();
        Assert.Equal(slug, created!.Slug);
        Assert.True(created.IsEnabled);
        Assert.Equal("{}", created.Configuration);

        var listResponse = await client.GetAsync('/' + ApiRoutes.PracticalModules.ModulesList);
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var modules = await listResponse.Content.ReadFromJsonAsync<PracticalModuleResponse[]>();
        Assert.Contains(modules!, module => module.Id == created.Id);
    }

    [Fact]
    public async Task Admin_CreatesModule_DuplicateSlug_Returns409()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Admin);
        var slug = "dup-" + Guid.NewGuid().ToString("N")[..8];
        var request = new CreatePracticalModuleRequest(
            slug, "Модуль", "Описание", "SQL_SIMULATOR", "/modules/dup", "dup-api", null);

        var first = await client.PostAsJsonAsync('/' + ApiRoutes.PracticalModules.ModulesList, request);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await client.PostAsJsonAsync('/' + ApiRoutes.PracticalModules.ModulesList, request);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Admin_UpdatesModule_SlugUnchanged()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Admin);
        var slug = "upd-" + Guid.NewGuid().ToString("N")[..8];
        var created = await CreateModuleAsync(client, slug);

        var updateResponse = await client.PutAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ForModule(created.Id),
            new UpdatePracticalModuleRequest(
                "Новое имя", "Новое описание", "SQL_SIMULATOR", "/modules/upd", "upd-api-v2", "{\"x\":1}", false));

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updated = await updateResponse.Content.ReadFromJsonAsync<PracticalModuleResponse>();
        Assert.Equal(slug, updated!.Slug);
        Assert.Equal("Новое имя", updated.Name);
        Assert.False(updated.IsEnabled);
        Assert.Equal("{\"x\":1}", updated.Configuration);
    }

    [Fact]
    public async Task Admin_UpdatesUnknownModule_Returns404()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Admin);

        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ForModule(Guid.NewGuid()),
            new UpdatePracticalModuleRequest("Имя", "Описание", "SQL_SIMULATOR", "/modules/x", "x-api", null, true));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Admin_DeletesModule()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Admin);
        var slug = "del-" + Guid.NewGuid().ToString("N")[..8];
        var created = await CreateModuleAsync(client, slug);

        var deleteResponse = await client.DeleteAsync('/' + ApiRoutes.PracticalModules.ForModule(created.Id));
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var secondDelete = await client.DeleteAsync('/' + ApiRoutes.PracticalModules.ForModule(created.Id));
        Assert.Equal(HttpStatusCode.NotFound, secondDelete.StatusCode);
    }

    [Fact]
    public async Task Admin_CreatesModule_InvalidSlug_ReturnsValidationError()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Admin);

        var response = await client.PostAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ModulesList,
            new CreatePracticalModuleRequest(
                "Not A Valid Slug!", "Модуль", "Описание", "SQL_SIMULATOR", "/modules/x", "x-api", null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(EducationRoles.Teacher)]
    [InlineData(EducationRoles.Student)]
    public async Task NonAdmin_CannotUsePracticalModulesEndpoint(string role)
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(role);

        var response = await client.GetAsync('/' + ApiRoutes.PracticalModules.ModulesList);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Teacher_ListsEnabledModulesOnly()
    {
        var admin = factory.CreateClient();
        admin.AuthenticateAs(EducationRoles.Admin);
        var enabled = await CreateModuleAsync(admin, "en-" + Guid.NewGuid().ToString("N")[..8]);
        var toDisable = await CreateModuleAsync(admin, "dis-" + Guid.NewGuid().ToString("N")[..8]);
        var disableResponse = await admin.PutAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ForModule(toDisable.Id),
            new UpdatePracticalModuleRequest(
                toDisable.Name, toDisable.Description, toDisable.PracticeType,
                toDisable.BasePath, toDisable.IdentityAudience, toDisable.Configuration, false));
        disableResponse.EnsureSuccessStatusCode();

        var teacher = factory.CreateClient();
        teacher.AuthenticateAs(EducationRoles.Teacher);
        var response = await teacher.GetAsync('/' + ApiRoutes.PracticalModules.EnabledModulesList);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var modules = await response.Content.ReadFromJsonAsync<PracticalModuleResponse[]>();
        Assert.Contains(modules!, module => module.Id == enabled.Id);
        Assert.DoesNotContain(modules!, module => module.Id == toDisable.Id);
        Assert.All(modules!, module => Assert.True(module.IsEnabled));
    }

    [Fact]
    public async Task Student_CannotListEnabledModules()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);

        var response = await client.GetAsync('/' + ApiRoutes.PracticalModules.EnabledModulesList);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static async Task<PracticalModuleResponse> CreateModuleAsync(HttpClient client, string slug)
    {
        var response = await client.PostAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ModulesList,
            new CreatePracticalModuleRequest(
                slug, "Модуль", "Описание", "SQL_SIMULATOR", "/modules/" + slug, slug + "-api", null));

        return (await response.Content.ReadFromJsonAsync<PracticalModuleResponse>())!;
    }
}
