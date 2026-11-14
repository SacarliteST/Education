using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Education.Application.PracticalModules;
using Education.Contracts;
using Education.Contracts.Grades;
using Education.Domain.Courses;
using Education.Domain.PracticalModules;
using Education.Domain.Practicals;
using Education.Domain.Tests;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Education.Web.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Education.Tests.Grades;

public sealed class GradebookClassWindowApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public GradebookClassWindowApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Gradebook_WithClassWindow_UsesOnlyAttemptsStartedDuringWindow()
    {
        var scenario = await CreateInternalScenarioAsync();
        var client = CreateTeacherClient();

        var scoped = await client.GetFromJsonAsync<CourseGradebookResponse>(
            '/' + ApiRoutes.Grades.ForCourseGradebook(factory.Seed.OwnCourseId)
            + $"?classWindowId={scenario.WindowId:D}");
        var allTime = await client.GetFromJsonAsync<CourseGradebookResponse>(
            '/' + ApiRoutes.Grades.ForCourseGradebook(factory.Seed.OwnCourseId));

        var scopedCell = FindCell(scoped!, scenario.PracticalId);
        var allTimeCell = FindCell(allTime!, scenario.PracticalId);
        Assert.Equal(2, scopedCell.Grade);
        Assert.Equal(5, allTimeCell.Grade);
    }

    [Fact]
    public async Task Gradebook_WithWindowOfAnotherCourse_ReturnsStructuredBadRequest()
    {
        var foreignWindowId = await CreateWindowAsync(factory.Seed.OtherCourseId);
        var client = CreateTeacherClient();

        var response = await client.GetAsync(
            '/' + ApiRoutes.Grades.ForCourseGradebook(factory.Seed.OwnCourseId)
            + $"?classWindowId={foreignWindowId:D}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("ClassWindow.CourseMismatch", await ReadCodeAsync(response));
    }

    [Fact]
    public async Task ExternalResults_ReturnAssignmentsSessionsAndCatalogNames()
    {
        var scenario = await CreateExternalScenarioAsync();
        var client = CreateTeacherClient(new FakeCatalogClient(_ =>
        [
            new ModuleCatalogTask(scenario.ExternalTaskRef, "Соединение таблиц", "Проверка JOIN"),
        ]));

        var response = await client.GetFromJsonAsync<StudentPracticalResultsResponse>(
            '/' + ApiRoutes.Grades.ForStudentPracticalResults(scenario.PracticalId, factory.Seed.TestUserId)
            + $"?classWindowId={scenario.WindowId:D}");

        Assert.Equal("external", response!.Kind);
        Assert.Equal(scenario.WindowId, response.ClassWindowId);
        Assert.Empty(response.TestAttempts);
        var task = Assert.Single(response.Tasks);
        Assert.Equal(scenario.AssignmentId, task.AssignmentId);
        Assert.Null(task.PracticalTaskId);
        Assert.Equal("Соединение таблиц", task.TaskName);
        var session = Assert.Single(task.Sessions);
        Assert.Equal(scenario.SessionId, session.SessionId);
        Assert.Equal("COMPLETED", session.Status);
        Assert.Equal(100, session.Grade);
    }

    [Fact]
    public async Task ExternalResults_WhenCatalogUnavailable_ReturnReferenceWithoutName()
    {
        var scenario = await CreateExternalScenarioAsync();
        var client = CreateTeacherClient(new FakeCatalogClient(_ =>
            throw new ModuleCatalogUnavailableException("Каталог недоступен.")));

        var response = await client.GetFromJsonAsync<StudentPracticalResultsResponse>(
            '/' + ApiRoutes.Grades.ForStudentPracticalResults(scenario.PracticalId, factory.Seed.TestUserId)
            + $"?classWindowId={scenario.WindowId:D}");

        var task = Assert.Single(response!.Tasks);
        Assert.Equal(scenario.ExternalTaskRef, task.ExternalTaskRef);
        Assert.Null(task.TaskName);
    }

    [Fact]
    public async Task FixedExternalResults_FilterSessionsByStartTime()
    {
        var scenario = await CreateFixedExternalScenarioAsync();
        var client = CreateTeacherClient(new FakeCatalogClient(_ =>
        [
            new ModuleCatalogTask(scenario.ExternalTaskRef, "Фиксированное задание", String.Empty),
        ]));

        var response = await client.GetFromJsonAsync<StudentPracticalResultsResponse>(
            '/' + ApiRoutes.Grades.ForStudentPracticalResults(scenario.PracticalId, factory.Seed.TestUserId)
            + $"?classWindowId={scenario.WindowId:D}");

        var task = Assert.Single(response!.Tasks);
        Assert.Null(task.AssignmentId);
        Assert.Equal(scenario.PracticalTaskId, task.PracticalTaskId);
        Assert.Equal("Фиксированное задание", task.TaskName);
        var session = Assert.Single(task.Sessions);
        Assert.Equal(scenario.InsideSessionId, session.SessionId);
        Assert.Equal(50, session.Grade);
    }

    [Fact]
    public async Task InternalResults_FilterAttemptsByWindow()
    {
        var scenario = await CreateInternalScenarioAsync();
        var client = CreateTeacherClient();

        var response = await client.GetFromJsonAsync<StudentPracticalResultsResponse>(
            '/' + ApiRoutes.Grades.ForStudentPracticalResults(scenario.PracticalId, factory.Seed.TestUserId)
            + $"?classWindowId={scenario.WindowId:D}");

        Assert.Equal("internal", response!.Kind);
        Assert.Empty(response.Tasks);
        var attempt = Assert.Single(response.TestAttempts);
        Assert.Equal(scenario.InsideAttemptId, attempt.TestResultId);
        Assert.True(attempt.IsCompleted);
    }

    [Fact]
    public async Task Results_ForStudentNotAssignedToCourse_Return404()
    {
        var client = CreateTeacherClient();

        var response = await client.GetAsync('/' + ApiRoutes.Grades.ForStudentPracticalResults(
            factory.Seed.UnassignedPracticalId,
            factory.Seed.TestUserId));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Student.NotAssignedToCourse", await ReadCodeAsync(response));
    }

    private HttpClient CreateTeacherClient(IModuleCatalogClient? catalog = null)
    {
        var application = catalog is null
            ? factory
            : factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IModuleCatalogClient>();
                services.AddScoped(_ => catalog);
            }));
        var client = application.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);
        return client;
    }

    private async Task<InternalScenario> CreateInternalScenarioAsync()
    {
        var startsAt = DateTimeOffset.UtcNow.AddMinutes(-20);
        var window = ClassWindow.Start(
            factory.Seed.OwnCourseId,
            "GRADE-" + Guid.NewGuid().ToString("N"),
            [],
            startsAt,
            40,
            factory.Seed.TestUserId);
        var practical = new PracticalMaterial(factory.Seed.OwnModuleId, "Window grade " + Guid.NewGuid().ToString("N"));

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        db.ClassWindows.Add(window);
        db.PracticalMaterials.Add(practical);
        await db.SaveChangesAsync();

        var before = new TestResult(factory.Seed.TestUserId, practical.Id, 1);
        before.Complete(1, 1, startsAt.AddMinutes(-10).UtcDateTime);
        var inside = new TestResult(factory.Seed.TestUserId, practical.Id, 2);
        inside.Complete(0, 1, startsAt.AddMinutes(5).UtcDateTime);
        db.TestResults.AddRange(before, inside);
        db.Entry(before).Property(result => result.StatedDate).CurrentValue = startsAt.AddMinutes(-10).UtcDateTime;
        db.Entry(inside).Property(result => result.StatedDate).CurrentValue = startsAt.AddMinutes(5).UtcDateTime;
        await db.SaveChangesAsync();

        return new InternalScenario(window.Id, practical.Id, inside.Id);
    }

    private async Task<ExternalScenario> CreateExternalScenarioAsync()
    {
        var startsAt = DateTimeOffset.UtcNow.AddMinutes(-10);
        var module = new PracticalModule(
            "grade-" + Guid.NewGuid().ToString("N"),
            "SQL module",
            "Test module",
            "SQL_SIMULATOR",
            "/modules/sql",
            "sql-module-api",
            "{\"catalogEndpoint\":\"http://sql-module/tasks\"}");
        var practical = new PracticalMaterial(factory.Seed.OwnModuleId, "External grade " + Guid.NewGuid().ToString("N"));
        practical.BindExternalModule(3, 60);
        var window = ClassWindow.Start(
            factory.Seed.OwnCourseId,
            "EXTERNAL-" + Guid.NewGuid().ToString("N"),
            [],
            startsAt,
            30,
            factory.Seed.TestUserId);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        db.PracticalModules.Add(module);
        db.PracticalMaterials.Add(practical);
        db.ClassWindows.Add(window);
        await db.SaveChangesAsync();

        var externalTaskRef = "task-" + Guid.NewGuid().ToString("N");
        var rule = new TaskSelectionRule(practical.Id, module.Id, 1);
        var assignment = new PracticalTaskAssignment(
            practical.Id,
            factory.Seed.TestUserId,
            window.Id,
            externalTaskRef,
            0,
            startsAt);
        db.TaskSelectionRules.Add(rule);
        db.PracticalTaskAssignments.Add(assignment);
        await db.SaveChangesAsync();

        var session = new PracticalModuleSession(
            Guid.NewGuid(),
            null,
            factory.Seed.TestUserId,
            1,
            "session-key",
            "/return",
            startsAt.AddMinutes(2),
            startsAt.AddMinutes(20),
            assignment.Id);
        session.Complete(startsAt.AddMinutes(3), 100, null);
        db.PracticalModuleSessions.Add(session);
        await db.SaveChangesAsync();

        return new ExternalScenario(window.Id, practical.Id, assignment.Id, session.Id, externalTaskRef);
    }

    private async Task<FixedExternalScenario> CreateFixedExternalScenarioAsync()
    {
        var startsAt = DateTimeOffset.UtcNow.AddMinutes(-10);
        var module = new PracticalModule(
            "fixed-grade-" + Guid.NewGuid().ToString("N"),
            "SQL module",
            "Test module",
            "SQL_SIMULATOR",
            "/modules/sql",
            "sql-module-api",
            "{\"catalogEndpoint\":\"http://sql-module/tasks\"}");
        var practical = new PracticalMaterial(factory.Seed.OwnModuleId, "Fixed grade " + Guid.NewGuid().ToString("N"));
        practical.BindExternalModule(3, 60);
        var externalTaskRef = "fixed-task-" + Guid.NewGuid().ToString("N");
        var task = new Case(practical.Id, "Fixed task", "Solve it");
        task.LinkExternalTask(module.Id, externalTaskRef);
        var window = ClassWindow.Start(
            factory.Seed.OwnCourseId,
            "FIXED-" + Guid.NewGuid().ToString("N"),
            [],
            startsAt,
            30,
            factory.Seed.TestUserId);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        db.PracticalModules.Add(module);
        db.PracticalMaterials.Add(practical);
        db.Cases.Add(task);
        db.ClassWindows.Add(window);
        await db.SaveChangesAsync();

        var before = new PracticalModuleSession(
            Guid.NewGuid(),
            task.Id,
            factory.Seed.TestUserId,
            1,
            "before-key",
            "/return",
            startsAt.AddMinutes(-5),
            startsAt);
        before.Complete(startsAt.AddMinutes(-4), 100, null);
        var inside = new PracticalModuleSession(
            Guid.NewGuid(),
            task.Id,
            factory.Seed.TestUserId,
            2,
            "inside-key",
            "/return",
            startsAt.AddMinutes(5),
            startsAt.AddMinutes(20));
        inside.Complete(startsAt.AddMinutes(6), 50, null);
        db.PracticalModuleSessions.AddRange(before, inside);
        await db.SaveChangesAsync();

        return new FixedExternalScenario(window.Id, practical.Id, task.Id, inside.Id, externalTaskRef);
    }

    private async Task<Guid> CreateWindowAsync(Guid courseId)
    {
        var window = ClassWindow.Start(
            courseId,
            "FOREIGN-" + Guid.NewGuid().ToString("N"),
            [],
            DateTimeOffset.UtcNow.AddMinutes(-5),
            30,
            factory.Seed.OtherTeacherId);
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        db.ClassWindows.Add(window);
        await db.SaveChangesAsync();
        return window.Id;
    }

    private static GradebookCellResponse FindCell(CourseGradebookResponse response, Guid practicalId)
    {
        var student = Assert.Single(response.Students, item => item.StudentId == TestAuthHandler.TestUserId);
        return Assert.Single(student.Cells, item => item.PracticalId == practicalId);
    }

    private static async Task<string?> ReadCodeAsync(HttpResponseMessage response)
    {
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("code").GetString();
    }

    private sealed class FakeCatalogClient(Func<PracticalModule, IReadOnlyList<ModuleCatalogTask>> handler)
        : IModuleCatalogClient
    {
        public Task<IReadOnlyList<ModuleCatalogTask>> GetTasksAsync(
            PracticalModule module,
            CancellationToken cancellationToken = default) => Task.FromResult(handler(module));
    }

    private sealed record InternalScenario(Guid WindowId, Guid PracticalId, Guid InsideAttemptId);

    private sealed record ExternalScenario(
        Guid WindowId,
        Guid PracticalId,
        Guid AssignmentId,
        Guid SessionId,
        string ExternalTaskRef);

    private sealed record FixedExternalScenario(
        Guid WindowId,
        Guid PracticalId,
        Guid PracticalTaskId,
        Guid InsideSessionId,
        string ExternalTaskRef);
}
