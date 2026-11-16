using System.Net;
using System.Net.Http.Json;
using Education.Application.PracticalModules;
using Education.Application.Practicals.TaskAssignments;
using Education.Contracts;
using Education.Contracts.Practicals;
using Education.Domain.Courses;
using Education.Domain.PracticalModules;
using Education.Domain.Practicals;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Education.Tests.Support;
using Education.Web.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Education.Tests.Practicals;

public sealed class TaskSelectionDifficultyQuotaApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public TaskSelectionDifficultyQuotaApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Quotas_ArePersisted_AndGenerateExactDifficultyDistribution()
    {
        var catalog = new[]
        {
            CatalogTask("basic-1", 1),
            CatalogTask("basic-2", 1),
            CatalogTask("basic-extra", 1),
            CatalogTask("hard-1", 4),
            CatalogTask("hard-extra", 4),
        };
        var scenario = await CreateScenarioAsync();
        using var application = CreateApplication(new FakeCatalogClient(_ => catalog));
        var client = application.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);

        var response = await client.PutAsJsonAsync(
            RuleRoute(scenario.PracticalId),
            new UpsertTaskSelectionRuleRequest(
                scenario.ModuleId,
                3,
                [],
                [],
                [new TaskDifficultyQuota(1, 2), new TaskDifficultyQuota(4, 1)]));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var savedRule = await response.Content.ReadFromJsonAsync<TaskSelectionRuleResponse>();
        Assert.False(savedRule!.EmptySelectionWarning);
        Assert.Equal(
            [new TaskDifficultyQuota(1, 2), new TaskDifficultyQuota(4, 1)],
            savedRule.DifficultyQuotas);

        using var scope = application.Services.CreateScope();
        var assignmentService = scope.ServiceProvider.GetRequiredService<ITaskAssignmentService>();
        await assignmentService.GenerateForWindowAsync(scenario.WindowId);

        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var assignedRefs = await db.PracticalTaskAssignments
            .AsNoTracking()
            .Where(assignment => assignment.PracticalMaterialId == scenario.PracticalId)
            .Select(assignment => assignment.ExternalTaskRef)
            .ToListAsync();
        var assignedDifficulties = assignedRefs
            .Select(taskRef => catalog.Single(task => task.Ref == taskRef).DifficultyLevel)
            .ToList();

        Assert.Equal(3, assignedDifficulties.Count);
        Assert.Equal(2, assignedDifficulties.Count(level => level == 1));
        Assert.Single(assignedDifficulties, level => level == 4);
    }

    [Fact]
    public async Task QuotaSum_DifferentFromTaskCount_ReturnsValidationProblem()
    {
        var scenario = await CreateScenarioAsync();
        using var application = CreateApplication(new FakeCatalogClient(_ => []));
        var client = application.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);

        var response = await client.PutAsJsonAsync(
            RuleRoute(scenario.PracticalId),
            new UpsertTaskSelectionRuleRequest(
                scenario.ModuleId,
                3,
                [],
                [],
                [new TaskDifficultyQuota(1, 1), new TaskDifficultyQuota(3, 1)]));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task InsufficientCatalog_ReturnsWarning_AndDoesNotCreatePartialSet()
    {
        var scenario = await CreateScenarioAsync();
        using var application = CreateApplication(new FakeCatalogClient(_ => [CatalogTask("basic-1", 1)]));
        var client = application.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);

        var response = await client.PutAsJsonAsync(
            RuleRoute(scenario.PracticalId),
            new UpsertTaskSelectionRuleRequest(
                scenario.ModuleId,
                3,
                [],
                [],
                [new TaskDifficultyQuota(1, 2), new TaskDifficultyQuota(4, 1)]));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var savedRule = await response.Content.ReadFromJsonAsync<TaskSelectionRuleResponse>();
        Assert.True(savedRule!.EmptySelectionWarning);

        using var scope = application.Services.CreateScope();
        var assignmentService = scope.ServiceProvider.GetRequiredService<ITaskAssignmentService>();
        await assignmentService.GenerateForWindowAsync(scenario.WindowId);

        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        Assert.False(await db.PracticalTaskAssignments
            .AsNoTracking()
            .AnyAsync(assignment => assignment.PracticalMaterialId == scenario.PracticalId));
    }

    private async Task<Scenario> CreateScenarioAsync()
    {
        var module = new PracticalModule(
            "quota-" + Guid.NewGuid().ToString("N"),
            "SQL module",
            "Test module",
            "SQL_SIMULATOR",
            "/modules/sql",
            "sql-module-api",
            "{\"catalogEndpoint\":\"http://sql-module/tasks\"}");
        var practical = new PracticalMaterial(
            factory.Seed.OwnModuleId,
            "Difficulty quotas " + Guid.NewGuid().ToString("N"));
        var window = ClassWindow.Start(
            factory.Seed.OwnCourseId,
            null,
            [factory.Seed.TestUserId],
            DateTimeOffset.UtcNow,
            30,
            factory.Seed.TestUserId);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        db.PracticalModules.Add(module);
        db.PracticalMaterials.Add(practical);
        db.ClassWindows.Add(window);
        await db.SaveChangesAsync();

        return new Scenario(module.Id, practical.Id, window.Id);
    }

    private Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program> CreateApplication(
        IModuleCatalogClient catalog) =>
        factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IModuleCatalogClient>();
            services.AddSingleton<IModuleCatalogClient>(catalog);
        }));

    private static ModuleCatalogTask CatalogTask(string taskRef, short difficulty) =>
        new(taskRef, taskRef, String.Empty) { DifficultyLevel = difficulty };

    private static string RuleRoute(Guid practicalId) =>
        '/' + ApiRoutes.Practicals.TaskSelectionRule.Replace("{practicalId:guid}", practicalId.ToString());

    private sealed class FakeCatalogClient(Func<PracticalModule, IReadOnlyList<ModuleCatalogTask>> handler)
        : IModuleCatalogClient
    {
        public Task<IReadOnlyList<ModuleCatalogTask>> GetTasksAsync(
            PracticalModule module,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(handler(module));
    }

    private sealed record Scenario(Guid ModuleId, Guid PracticalId, Guid WindowId);
}
