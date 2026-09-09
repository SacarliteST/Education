using System.Net;
using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.PracticalModules;
using Education.Contracts.Practicals;
using Education.Domain.Practicals;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Education.Web.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Tests.PracticalModules;

public sealed class PracticalModuleBindingApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public PracticalModuleBindingApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Teacher_BindsModule_ToOwnPractical()
    {
        var moduleId = await RegisterModuleAsync();
        var practicalId = factory.Seed.UnassignedPracticalId; // чистая практика курса преподавателя

        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);
        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModule(practicalId),
            new BindPracticalModuleRequest(moduleId, "sql-join-001", 3, 90));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var practical = await db.PracticalMaterials.AsNoTracking().SingleAsync(p => p.Id == practicalId);
        Assert.Equal(PracticalKind.External, practical.Kind);
        Assert.Equal(3, practical.TriesCount);
        Assert.Equal(90, practical.TimeLimitMinutes);

        var cases = await db.Cases.AsNoTracking().Where(c => c.PracticalMaterialId == practicalId).ToListAsync();
        Assert.Single(cases);
        Assert.Equal(moduleId, cases[0].PracticalModuleId);
        Assert.Equal("sql-join-001", cases[0].ExternalTaskRef);
    }

    [Fact]
    public async Task Bind_PracticalWithStudentActivity_Returns409()
    {
        var moduleId = await RegisterModuleAsync();
        // AssignedStartPracticalId имеет CaseFile от другого студента → защита срабатывает
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);

        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModule(factory.Seed.AssignedStartPracticalId),
            new BindPracticalModuleRequest(moduleId, "sql-join-001", 1, null));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task PracticalDetail_ReflectsExternalBinding()
    {
        var moduleId = await RegisterModuleAsync();
        var practicalId = factory.Seed.SubmitPracticalId; // чистая, курс преподавателя

        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);
        var bind = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModule(practicalId),
            new BindPracticalModuleRequest(moduleId, "sql-join-001", 3, 90));
        bind.EnsureSuccessStatusCode();

        var detail = await client.GetFromJsonAsync<PracticalDetailResponse>(
            '/' + ApiRoutes.Practicals.ForPractical(practicalId));

        Assert.Equal("external", detail!.Kind);
        Assert.Equal(3, detail.TriesCount);
        Assert.Equal(90, detail.TimeLimitMinutes);
        Assert.NotNull(detail.ModuleBinding);
        Assert.Equal(moduleId, detail.ModuleBinding!.PracticalModuleId);
        Assert.Equal("sql-join-001", detail.ModuleBinding.ExternalTaskRef);
        Assert.NotEqual(Guid.Empty, detail.ModuleBinding.TaskId);
    }

    [Fact]
    public async Task PracticalDetail_Internal_HasNoBinding()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);

        var detail = await client.GetFromJsonAsync<PracticalDetailResponse>(
            '/' + ApiRoutes.Practicals.ForPractical(factory.Seed.LimitedPracticalId));

        Assert.Equal("internal", detail!.Kind);
        Assert.Null(detail.ModuleBinding);
    }

    [Fact]
    public async Task PracticalDetail_Unknown_Returns404()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);

        var response = await client.GetAsync('/' + ApiRoutes.Practicals.ForPractical(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Teacher_CannotBind_OtherTeacherPractical()
    {
        var moduleId = await RegisterModuleAsync();

        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);
        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModule(factory.Seed.OtherTeacherPracticalId),
            new BindPracticalModuleRequest(moduleId, "ref", 1, null));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Bind_UnknownModule_Returns404()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);
        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModule(factory.Seed.AssignedStartPracticalId),
            new BindPracticalModuleRequest(Guid.NewGuid(), "ref", 1, null));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Bind_EmptyRef_ReturnsValidationProblem()
    {
        var moduleId = await RegisterModuleAsync();

        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);
        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModule(factory.Seed.AssignedStartPracticalId),
            new BindPracticalModuleRequest(moduleId, "", 1, null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Student_Forbidden()
    {
        var moduleId = await RegisterModuleAsync();

        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);
        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModule(factory.Seed.AssignedStartPracticalId),
            new BindPracticalModuleRequest(moduleId, "ref", 1, null));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private async Task<Guid> RegisterModuleAsync()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Admin);
        var slug = "sql-" + Guid.NewGuid().ToString("N")[..8];

        var response = await client.PostAsJsonAsync(
            '/' + ApiRoutes.PracticalModules.ModulesList,
            new CreatePracticalModuleRequest(
                slug, "SQL", "desc", "SQL_SIMULATOR", "/modules/sql", "sql-module-api", null));

        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<PracticalModuleResponse>();
        return created!.Id;
    }
}
