using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Education.Application.PracticalModules;
using Education.Contracts;
using Education.Contracts.ClassWindows;
using Education.Contracts.PracticalModules;
using Education.Contracts.Practicals;
using Education.Domain.Courses;
using Education.Domain.Practicals;
using Education.Domain.Users;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Education.Tests.Support;
using Education.Web.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Education.Tests.ClassWindows;

public sealed class ClassWindowsApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public ClassWindowsApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task ModeOff_StudentAlwaysHasAccess_AndSessionStarts()
    {
        var context = await CreateCourseAsync(practiceOnly: false, group: "G-OFF");
        var push = new FakeModulePushClient();

        var access = await GetAccessAsync(context.CourseId);
        Assert.False(access.PracticeOnlyDuringClass);
        Assert.True(access.IsOpen);

        var response = await StartSessionAsync(push, context);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Null(push.Pushes.Single().ExpiresAt);
        Assert.Equal(3, push.Pushes.Single().MaxAttempts);
    }

    [Fact]
    public async Task ModeOn_WithoutWindow_BlocksSessionStart_AndTestStart_With403Code()
    {
        var context = await CreateCourseAsync(practiceOnly: true, group: "G-CLOSED");

        var access = await GetAccessAsync(context.CourseId);
        Assert.True(access.PracticeOnlyDuringClass);
        Assert.False(access.IsOpen);

        var session = await StartSessionAsync(new FakeModulePushClient(), context);
        Assert.Equal(HttpStatusCode.Forbidden, session.StatusCode);
        Assert.Equal("ClassWindow.Closed", await ReadCodeAsync(session));

        var student = factory.CreateClient();
        student.AuthenticateAs(EducationRoles.Student);
        var start = await student.PutAsync('/' + ApiRoutes.TestResults.ForStart(context.PracticalId), null);
        Assert.Equal(HttpStatusCode.Forbidden, start.StatusCode);
        Assert.Equal("ClassWindow.Closed", await ReadCodeAsync(start));
    }

    [Fact]
    public async Task ModeOn_StudentWithoutGroup_IsNeverInWindow()
    {
        var context = await CreateCourseAsync(practiceOnly: true, group: null);
        await StartWindowAsync(context.CourseId, "G-X", 60);

        var access = await GetAccessAsync(context.CourseId);
        Assert.False(access.IsOpen);
    }

    [Fact]
    public async Task Teacher_StartsWindow_StudentGetsAccess_AndExpiryIsMinOfWindowAndLimit()
    {
        var context = await CreateCourseAsync(practiceOnly: true, group: "G-OPEN", timeLimitMinutes: 120);
        var started = await StartWindowAsync(context.CourseId, "g-open ", 30);
        Assert.True(started.IsActive);

        var access = await GetAccessAsync(context.CourseId);
        Assert.True(access.IsOpen);
        Assert.NotNull(access.EndsAt);

        var push = new FakeModulePushClient();
        var response = await StartSessionAsync(push, context);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var expires = push.Pushes.Single().ExpiresAt;
        Assert.NotNull(expires);
        // окно (30 мин) короче лимита практики (120 мин)
        Assert.True(expires <= started.EndsAt.AddSeconds(1));
        Assert.True(expires > DateTimeOffset.UtcNow.AddMinutes(25));
    }

    [Fact]
    public async Task Start_WhenWindowAlreadyOpen_Returns409()
    {
        var context = await CreateCourseAsync(practiceOnly: true, group: "G-DUP");
        await StartWindowAsync(context.CourseId, "G-DUP", 30);

        var teacher = TeacherClient();
        var second = await teacher.PostAsJsonAsync(
            '/' + ApiRoutes.Courses.ForClassWindows(context.CourseId),
            new StartClassWindowRequest("G-DUP", 30));
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        Assert.Equal("ClassWindow.AlreadyOpen", await ReadCodeAsync(second));
    }

    [Fact]
    public async Task Start_InvalidDuration_Returns400()
    {
        var context = await CreateCourseAsync(practiceOnly: true, group: "G-BAD");
        var teacher = TeacherClient();
        var response = await teacher.PostAsJsonAsync(
            '/' + ApiRoutes.Courses.ForClassWindows(context.CourseId),
            new StartClassWindowRequest("G-BAD", 3));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Extend_MovesEnd_AndRepushesActiveSession()
    {
        var context = await CreateCourseAsync(practiceOnly: true, group: "G-EXT");
        var window = await StartWindowAsync(context.CourseId, "G-EXT", 20);
        var push = new FakeModulePushClient();
        Assert.Equal(HttpStatusCode.OK, (await StartSessionAsync(push, context)).StatusCode);

        var teacher = TeacherClientWith(push);
        var extended = await teacher.PostAsJsonAsync(
            '/' + ApiRoutes.Courses.ForClassWindowExtend(window.Id), new ExtendClassWindowRequest(15));
        Assert.Equal(HttpStatusCode.OK, extended.StatusCode);
        var body = await extended.Content.ReadFromJsonAsync<ClassWindowResponse>();
        Assert.True((window.EndsAt.AddMinutes(15) - body!.EndsAt).Duration() < TimeSpan.FromMilliseconds(1));

        Assert.Equal(2, push.Pushes.Count);
        Assert.Equal(body.EndsAt, push.Pushes[1].ExpiresAt);
        Assert.Equal(push.Pushes[0].SessionId, push.Pushes[1].SessionId);
    }

    [Fact]
    public async Task Finish_ClosesAccess_AndPushesImmediateExpiry()
    {
        var context = await CreateCourseAsync(practiceOnly: true, group: "G-FIN");
        var window = await StartWindowAsync(context.CourseId, "G-FIN", 60);
        var push = new FakeModulePushClient();
        Assert.Equal(HttpStatusCode.OK, (await StartSessionAsync(push, context)).StatusCode);

        var teacher = TeacherClientWith(push);
        var finished = await teacher.PostAsync('/' + ApiRoutes.Courses.ForClassWindowFinish(window.Id), null);
        Assert.Equal(HttpStatusCode.OK, finished.StatusCode);
        var body = await finished.Content.ReadFromJsonAsync<ClassWindowResponse>();
        Assert.False(body!.IsActive);

        Assert.Equal(2, push.Pushes.Count);
        Assert.True(push.Pushes[1].ExpiresAt <= DateTimeOffset.UtcNow.AddSeconds(2));

        var access = await GetAccessAsync(context.CourseId);
        Assert.False(access.IsOpen);

        var again = await teacher.PostAsync('/' + ApiRoutes.Courses.ForClassWindowFinish(window.Id), null);
        Assert.Equal(HttpStatusCode.Conflict, again.StatusCode);
        Assert.Equal("ClassWindow.AlreadyFinished", await ReadCodeAsync(again));

        // новая пара для той же группы после завершения возможна
        var next = await StartWindowAsync(context.CourseId, "G-FIN", 10);
        Assert.True(next.IsActive);
    }

    [Fact]
    public async Task OtherGroup_DoesNotGetAccess()
    {
        var context = await CreateCourseAsync(practiceOnly: true, group: "G-MINE");
        await StartWindowAsync(context.CourseId, "G-OTHER", 60);
        Assert.False((await GetAccessAsync(context.CourseId)).IsOpen);
    }

    [Fact]
    public async Task OtherTeacher_CannotManageWindows()
    {
        var context = await CreateCourseAsync(practiceOnly: true, group: "G-SEC");
        var teacher = TeacherClient();
        teacher.AuthenticateAsIdentity(factory.Seed.OtherTeacherId, [EducationRoles.Teacher]);
        var response = await teacher.PostAsJsonAsync(
            '/' + ApiRoutes.Courses.ForClassWindows(context.CourseId),
            new StartClassWindowRequest("G-SEC", 30));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Teacher_ReadsGroupsAndHistory_AndTogglesMode()
    {
        var context = await CreateCourseAsync(practiceOnly: false, group: "G-LIST");
        await StartWindowAsync(context.CourseId, "G-LIST", 30);

        var teacher = TeacherClient();
        var overview = await teacher.GetFromJsonAsync<CourseClassWindowsResponse>(
            '/' + ApiRoutes.Courses.ForClassWindows(context.CourseId));
        Assert.False(overview!.PracticeOnlyDuringClass);
        Assert.Contains("G-LIST", overview.Groups);
        Assert.Single(overview.Windows);

        var set = await teacher.PutAsJsonAsync(
            '/' + ApiRoutes.Courses.ForPracticeAccess(context.CourseId), new SetPracticeAccessRequest(true));
        Assert.Equal(HttpStatusCode.NoContent, set.StatusCode);
        var after = await teacher.GetFromJsonAsync<CourseClassWindowsResponse>(
            '/' + ApiRoutes.Courses.ForClassWindows(context.CourseId));
        Assert.True(after!.PracticeOnlyDuringClass);
    }

    private HttpClient TeacherClient()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);
        return client;
    }

    private HttpClient TeacherClientWith(IModulePushClient push)
    {
        var client = factory.WithModuleFakes(push).CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);
        return client;
    }

    private async Task<HttpResponseMessage> StartSessionAsync(FakeModulePushClient push, CourseContext context)
    {
        var client = factory.WithModuleFakes(push).CreateClient();
        client.AuthenticateAs(EducationRoles.Student);
        return await client.PostAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModuleSessions(context.PracticalId),
            new StartModuleSessionRequest(context.TaskId));
    }

    private async Task<ClassAccessResponse> GetAccessAsync(Guid courseId)
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);
        return (await client.GetFromJsonAsync<ClassAccessResponse>('/' + ApiRoutes.Courses.ForClassAccess(courseId)))!;
    }

    private async Task<ClassWindowResponse> StartWindowAsync(Guid courseId, string group, int minutes)
    {
        var response = await TeacherClient().PostAsJsonAsync(
            '/' + ApiRoutes.Courses.ForClassWindows(courseId), new StartClassWindowRequest(group, minutes));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClassWindowResponse>())!;
    }

    private static async Task<string?> ReadCodeAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.TryGetProperty("code", out var code) ? code.GetString() : null;
    }

    /// <summary>Свой курс с внешней практикой на каждый тест; студенту (тот же тестовый пользователь) задаётся группа.</summary>
    private async Task<CourseContext> CreateCourseAsync(bool practiceOnly, string? group, int? timeLimitMinutes = null)
    {
        Guid courseId, practicalId, taskId;
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
            var course = new Course("cw " + Guid.NewGuid().ToString("N")[..6], "d", DateTimeOffset.UtcNow, factory.Seed.TestUserId);
            course.SetPracticeOnlyDuringClass(practiceOnly);
            await db.Courses.AddAsync(course);
            await db.CourseBindUsers.AddAsync(new CourseBindUser(course.Id, factory.Seed.TestUserId));
            var module = new Module(course.Id, "m");
            await db.Modules.AddAsync(module);
            var practical = new PracticalMaterial(module.Id, "p");
            await db.PracticalMaterials.AddAsync(practical);
            await db.PracticalBindUsers.AddAsync(new PracticalBindUser(practical.Id, factory.Seed.TestUserId));
            var user = await db.Users.SingleAsync(item => item.Id == factory.Seed.TestUserId);
            user.ChangeGroup(group);
            await db.SaveChangesAsync();
            courseId = course.Id;
            practicalId = practical.Id;
            taskId = Guid.Empty;
        }

        var moduleId = await factory.RegisterModuleAsync();

        var bind = await TeacherClient().PutAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModule(practicalId),
            new BindPracticalModuleRequest(moduleId, "sql-join-001", 3, timeLimitMinutes, AttemptsPerSession: 3));
        bind.EnsureSuccessStatusCode();

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
            taskId = await db.Cases
                .Where(item => item.PracticalMaterialId == practicalId && item.PracticalModuleId != null)
                .Select(item => item.Id)
                .SingleAsync();
        }

        return new CourseContext(courseId, practicalId, taskId);
    }

    private sealed record CourseContext(Guid CourseId, Guid PracticalId, Guid TaskId);
}
