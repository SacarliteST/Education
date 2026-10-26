using System.Net;
using System.Net.Http.Json;
using Education.Application.PracticalModules;
using Education.Domain.PracticalModules;
using Education.Contracts;
using Education.Contracts.Grades;
using Education.Contracts.PracticalModules;
using Education.Contracts.Practicals;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Education.Tests.Support;
using Education.Web.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Education.Tests.PracticalModules;

public sealed class ModuleSessionsApiTests : IClassFixture<TestWebApplicationFactory>
{
    private const string DevServiceKey = "dev-sql-module-service-key-change-me";
    private readonly TestWebApplicationFactory factory;

    public ModuleSessionsApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Student_StartsSession_GetsLaunchUrl()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 2, timeLimitMinutes: 60);
        var push = new FakeModulePushClient();

        var client = StudentClient(push);
        var response = await client.PostAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModuleSessions(context.PracticalId),
            new StartModuleSessionRequest(context.TaskId));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<StartModuleSessionResponse>();
        Assert.False(body!.Resumed);
        Assert.Equal(1, body.TryNumber);
        Assert.Contains($"/launch?session={body.SessionId}", body.LaunchUrl);
        // токен обменян с непустым session_id (fake кодирует его в значение)
        Assert.Contains($"#access_token=fake-token-{body.SessionId:N}", body.LaunchUrl);

        Assert.Single(push.Pushes);
        Assert.Equal(context.ExternalTaskRef, push.Pushes[0].TaskRef);
        Assert.False(String.IsNullOrEmpty(push.Pushes[0].SessionKey));
        Assert.DoesNotContain(push.Pushes[0].SessionKey, body.LaunchUrl);
        // Пуш адресует владельца по identity-id (== sub токена); учебный id пользователя теперь тот же.
        Assert.Equal(Auth.TestAuthHandler.TestUserId, push.Pushes[0].UserId);
        Assert.Equal(factory.Seed.TestUserId, push.Pushes[0].UserId);
    }

    [Fact]
    public async Task Start_Again_WhileActive_Resumes()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 2, timeLimitMinutes: null);
        var push = new FakeModulePushClient();
        var client = StudentClient(push);

        var first = await StartAsync(client, context);
        var second = await StartAsync(client, context);

        Assert.False(first.Resumed);
        Assert.True(second.Resumed);
        Assert.Equal(first.SessionId, second.SessionId);
        Assert.Equal(1, second.TryNumber);
        Assert.Equal(2, push.Pushes.Count);
    }

    [Fact]
    public async Task Resume_WhenModuleSessionAlreadyClosed_Returns409_SessionClosed()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 2, timeLimitMinutes: null);
        await StartAsync(StudentClient(new FakeModulePushClient()), context);

        var closing = new FakeModulePushClient { ThrowSessionClosed = true };
        var response = await StudentClient(closing).PostAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModuleSessions(context.PracticalId),
            new StartModuleSessionRequest(context.TaskId));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Contains("SessionClosed", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Start_WhenTriesExhausted_Returns409()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 1, timeLimitMinutes: null);
        var push = new FakeModulePushClient();
        var client = StudentClient(push);

        var started = await StartAsync(client, context);
        var abandon = await client.PostAsync(
            '/' + ApiRoutes.Practicals.ForModuleSessionAbandon(context.PracticalId, started.SessionId), null);
        Assert.Equal(HttpStatusCode.NoContent, abandon.StatusCode);

        var again = await client.PostAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModuleSessions(context.PracticalId),
            new StartModuleSessionRequest(context.TaskId));
        Assert.Equal(HttpStatusCode.Conflict, again.StatusCode);
    }

    [Fact]
    public async Task Start_ModuleUnavailable_Returns502_AndDoesNotBurnTry()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 1, timeLimitMinutes: null);
        var failing = new FakeModulePushClient { ThrowUnavailable = true };

        var down = await StudentClient(failing).PostAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModuleSessions(context.PracticalId),
            new StartModuleSessionRequest(context.TaskId));
        Assert.Equal(HttpStatusCode.BadGateway, down.StatusCode);

        var recovered = await StartAsync(StudentClient(new FakeModulePushClient()), context);
        Assert.Equal(1, recovered.TryNumber);
    }

    [Fact]
    public async Task Current_ReflectsGate()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 3, timeLimitMinutes: 45);
        var client = StudentClient(new FakeModulePushClient());

        var before = await client.GetFromJsonAsync<CurrentModuleSessionResponse>(
            '/' + ApiRoutes.Practicals.ForModuleSessionCurrent(context.PracticalId) + $"?taskId={context.TaskId}");
        Assert.Null(before!.Session);
        Assert.Equal(0, before.AttemptsCount);
        Assert.Equal(3, before.TriesCount);
        Assert.Equal(45, before.TimeLimitMinutes);

        await StartAsync(client, context);

        var after = await client.GetFromJsonAsync<CurrentModuleSessionResponse>(
            '/' + ApiRoutes.Practicals.ForModuleSessionCurrent(context.PracticalId) + $"?taskId={context.TaskId}");
        Assert.Equal("ACTIVE", after!.Session!.Status);
        Assert.Equal(1, after.AttemptsCount);
    }

    [Fact]
    public async Task Current_ShowsSubmissionsUsed_FromEventsJournal()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 3, timeLimitMinutes: null);
        var client = StudentClient(new FakeModulePushClient());
        var started = await StartAsync(client, context);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
            var now = DateTimeOffset.UtcNow;
            db.PracticalTaskEvents.AddRange(
                new PracticalTaskEvent(Guid.NewGuid(), started.SessionId, "sql_submit", now, "{}"),
                new PracticalTaskEvent(Guid.NewGuid(), started.SessionId, "sql_submit", now.AddSeconds(1), "{}"),
                new PracticalTaskEvent(Guid.NewGuid(), started.SessionId, "other", now.AddSeconds(2), "{}"));
            await db.SaveChangesAsync();
        }

        var current = await client.GetFromJsonAsync<CurrentModuleSessionResponse>(
            '/' + ApiRoutes.Practicals.ForModuleSessionCurrent(context.PracticalId) + $"?taskId={context.TaskId}");

        Assert.Equal(2, current!.SubmissionsCount);
        Assert.Null(current.AttemptsPerSession);
    }

    [Fact]
    public async Task Abandon_Twice_SecondIs409()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 3, timeLimitMinutes: null);
        var client = StudentClient(new FakeModulePushClient());
        var started = await StartAsync(client, context);

        var first = await client.PostAsync(
            '/' + ApiRoutes.Practicals.ForModuleSessionAbandon(context.PracticalId, started.SessionId), null);
        var second = await client.PostAsync(
            '/' + ApiRoutes.Practicals.ForModuleSessionAbandon(context.PracticalId, started.SessionId), null);

        Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Complete_WithServiceKey_SetsGrade_Idempotent()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 2, timeLimitMinutes: null);
        var client = StudentClient(new FakeModulePushClient());
        var started = await StartAsync(client, context);
        var sessionKey = await ReadSessionKeyAsync(started.SessionId);

        var complete1 = await SendCompleteAsync(started.SessionId, DevServiceKey, sessionKey, 100);
        var complete2 = await SendCompleteAsync(started.SessionId, DevServiceKey, sessionKey, 40);

        Assert.Equal(HttpStatusCode.OK, complete1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, complete2.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var session = await db.PracticalModuleSessions.AsNoTracking().SingleAsync(s => s.Id == started.SessionId);
        Assert.Equal(ModuleSessionState.Completed, session.Status);
        Assert.Equal(100, session.Grade);
    }

    [Fact]
    public async Task Complete_AfterExpiresAt_StillAcceptsLateGrade()
    {
        // Модуль может прислать итоговую (в т.ч. позднюю) оценку уже после
        // формального ExpiresAt — CompleteAsync не должен сам себе истекать
        // сессию прямо в этом вызове и тем самым отбрасывать легитимный результат.
        var context = await BindFreshExternalPracticalAsync(triesCount: 1, timeLimitMinutes: 1);
        var client = StudentClient(new FakeModulePushClient());
        var started = await StartAsync(client, context);
        var sessionKey = await ReadSessionKeyAsync(started.SessionId);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
            var session = await db.PracticalModuleSessions.SingleAsync(s => s.Id == started.SessionId);
            db.Entry(session).Property("ExpiresAt").CurrentValue = DateTimeOffset.UtcNow.AddMinutes(-10);
            await db.SaveChangesAsync();
        }

        var completed = await SendCompleteAsync(started.SessionId, DevServiceKey, sessionKey, 75);
        Assert.Equal(HttpStatusCode.OK, completed.StatusCode);

        using var verifyScope = factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var final = await verifyDb.PracticalModuleSessions.AsNoTracking().SingleAsync(s => s.Id == started.SessionId);
        Assert.Equal(ModuleSessionState.Completed, final.Status);
        Assert.Equal(75, final.Grade);
    }

    [Fact]
    public async Task ExternalGrade_BestOfN_ViaGradeEndpoint()
    {
        // назначенная студенту практика — GradesService требует назначения
        var context = await BindFreshExternalPracticalAsync(triesCount: 3, timeLimitMinutes: null, assignStudent: true);
        var client = StudentClient(new FakeModulePushClient());

        foreach (var grade in new[] { 60, 90, 40 })
        {
            var started = await StartAsync(client, context);
            var sessionKey = await ReadSessionKeyAsync(started.SessionId);
            var completed = await SendCompleteAsync(started.SessionId, DevServiceKey, sessionKey, grade);
            Assert.Equal(HttpStatusCode.OK, completed.StatusCode);
        }

        var result = await client.GetFromJsonAsync<PracticalGradeResponse>(
            '/' + ApiRoutes.Grades.ForPracticalGrade(context.PracticalId));

        // Лучший составной балл модуля (90 из 100) переводится в 5-балльную шкалу
        // теми же порогами практики, что и обычные тесты/кейсы (по умолчанию
        // 90/75/60), а не отдаётся студенту как есть.
        Assert.Equal(5, result!.Grade);
    }

    [Fact]
    public async Task Complete_BadServiceKey_401()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 1, timeLimitMinutes: null);
        var started = await StartAsync(StudentClient(new FakeModulePushClient()), context);
        var sessionKey = await ReadSessionKeyAsync(started.SessionId);

        var response = await SendCompleteAsync(started.SessionId, "wrong-key", sessionKey, 100);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Complete_UnknownSessionKey_401()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 1, timeLimitMinutes: null);
        var started = await StartAsync(StudentClient(new FakeModulePushClient()), context);

        var response = await SendCompleteAsync(started.SessionId, DevServiceKey, "not-the-session-key", 100);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Complete_AfterAbandon_409()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 1, timeLimitMinutes: null);
        var client = StudentClient(new FakeModulePushClient());
        var started = await StartAsync(client, context);
        var sessionKey = await ReadSessionKeyAsync(started.SessionId);
        await client.PostAsync(
            '/' + ApiRoutes.Practicals.ForModuleSessionAbandon(context.PracticalId, started.SessionId), null);

        var response = await SendCompleteAsync(started.SessionId, DevServiceKey, sessionKey, 100);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task NonStudent_Forbidden()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 1, timeLimitMinutes: null);

        var client = ClientWith(new FakeModulePushClient(), EducationRoles.Teacher);
        var response = await client.PostAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModuleSessions(context.PracticalId),
            new StartModuleSessionRequest(context.TaskId));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task SessionEvents_OwnerAndTeacher_See_Others_404()
    {
        // назначенная студенту практика под модулем OwnModuleId — курс принадлежит seeded-преподавателю
        var context = await BindFreshExternalPracticalAsync(triesCount: 2, timeLimitMinutes: null);
        var student = StudentClient(new FakeModulePushClient());
        var started = await StartAsync(student, context);

        await FeedEventAsync(started.SessionId, "sql_submit", "{\"sql\":\"SELECT 1\"}");
        await FeedEventAsync(started.SessionId, "sql_submit", "{\"sql\":\"SELECT 2\"}");

        var asStudent = await student.GetFromJsonAsync<ModuleSessionEventResponse[]>(
            '/' + ApiRoutes.Practicals.ForModuleSessionEvents(context.PracticalId, started.SessionId));
        Assert.Equal(2, asStudent!.Length);
        Assert.Equal("sql_submit", asStudent[0].Kind);

        var teacher = factory.CreateClient();
        teacher.AuthenticateAs(EducationRoles.Teacher);
        var asTeacher = await teacher.GetFromJsonAsync<ModuleSessionEventResponse[]>(
            '/' + ApiRoutes.Practicals.ForModuleSessionEvents(context.PracticalId, started.SessionId));
        Assert.Equal(2, asTeacher!.Length);

        var unknown = await student.GetAsync(
            '/' + ApiRoutes.Practicals.ForModuleSessionEvents(context.PracticalId, Guid.NewGuid()));
        Assert.Equal(HttpStatusCode.NotFound, unknown.StatusCode);
    }

    [Fact]
    public async Task SessionEvents_ReturnMarkdown_ToOwnerAndTeacher_NullWhenAbsent()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 2, timeLimitMinutes: null);
        var student = StudentClient(new FakeModulePushClient());
        var started = await StartAsync(student, context);

        await FeedEventAsync(started.SessionId, "sql_submit", "{\"sql\":\"SELECT 1\"}", "**Попытка 1 — неверно**");
        await FeedEventAsync(started.SessionId, "sql_submit", "{\"sql\":\"SELECT 2\"}");

        var url = '/' + ApiRoutes.Practicals.ForModuleSessionEvents(context.PracticalId, started.SessionId);
        var teacher = factory.CreateClient();
        teacher.AuthenticateAs(EducationRoles.Teacher);

        foreach (var client in new[] { student, teacher })
        {
            var events = await client.GetFromJsonAsync<ModuleSessionEventResponse[]>(url);
            Assert.Equal(2, events!.Length);
            Assert.Single(events, item => item.Markdown == "**Попытка 1 — неверно**");
            Assert.Single(events, item => item.Markdown is null);
        }
    }

    [Fact]
    public async Task ListSessions_Teacher_SeesAllAttempts_Student_Forbidden()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 3, timeLimitMinutes: null);
        var student = StudentClient(new FakeModulePushClient());

        var first = await StartAsync(student, context);
        await student.PostAsync(
            '/' + ApiRoutes.Practicals.ForModuleSessionAbandon(context.PracticalId, first.SessionId), null);
        var second = await StartAsync(student, context);

        var studentList = await student.GetAsync('/' + ApiRoutes.Practicals.ForModuleSessions(context.PracticalId));
        Assert.Equal(HttpStatusCode.Forbidden, studentList.StatusCode);

        var teacher = factory.CreateClient();
        teacher.AuthenticateAs(EducationRoles.Teacher);
        var rows = await teacher.GetFromJsonAsync<ModuleSessionSummaryResponse[]>(
            '/' + ApiRoutes.Practicals.ForModuleSessions(context.PracticalId));

        Assert.Equal(2, rows!.Length);
        Assert.Equal(second.SessionId, rows[0].SessionId);
        Assert.Contains(rows, row => row.SessionId == first.SessionId && row.Status == "EXPIRED");
        Assert.Contains(rows, row => row.SessionId == second.SessionId && row.Status == "ACTIVE");
        Assert.All(rows, row => Assert.False(String.IsNullOrWhiteSpace(row.StudentName)));
    }

    [Fact]
    public async Task ListSessions_AttemptsCount_FromCompletionData_NullWhenAbsent()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 3, timeLimitMinutes: null);
        var student = StudentClient(new FakeModulePushClient());

        var completedSession = await StartAsync(student, context);
        var completedKey = await ReadSessionKeyAsync(completedSession.SessionId);
        var completed = await SendCompleteAsync(
            completedSession.SessionId, DevServiceKey, completedKey, 80,
            new { totalAttempts = 3, correctAttemptId = (Guid?)null });
        Assert.Equal(HttpStatusCode.OK, completed.StatusCode);

        var activeSession = await StartAsync(student, context);

        var teacher = factory.CreateClient();
        teacher.AuthenticateAs(EducationRoles.Teacher);
        var rows = await teacher.GetFromJsonAsync<ModuleSessionSummaryResponse[]>(
            '/' + ApiRoutes.Practicals.ForModuleSessions(context.PracticalId));

        Assert.Equal(3, rows!.Single(row => row.SessionId == completedSession.SessionId).AttemptsCount);
        Assert.Null(rows!.Single(row => row.SessionId == activeSession.SessionId).AttemptsCount);
    }

    [Fact]
    public async Task ListSessions_MalformedCompletionData_AttemptsCountNull_NoError()
    {
        var context = await BindFreshExternalPracticalAsync(triesCount: 3, timeLimitMinutes: null);
        var student = StudentClient(new FakeModulePushClient());
        var started = await StartAsync(student, context);
        var key = await ReadSessionKeyAsync(started.SessionId);
        var completed = await SendCompleteAsync(
            started.SessionId, DevServiceKey, key, 60, new { totalAttempts = "много" });
        Assert.Equal(HttpStatusCode.OK, completed.StatusCode);

        var teacher = factory.CreateClient();
        teacher.AuthenticateAs(EducationRoles.Teacher);
        var rows = await teacher.GetFromJsonAsync<ModuleSessionSummaryResponse[]>(
            '/' + ApiRoutes.Practicals.ForModuleSessions(context.PracticalId));

        Assert.Null(rows!.Single(row => row.SessionId == started.SessionId).AttemptsCount);
    }

    private async Task FeedEventAsync(Guid sessionId, string kind, string payloadJson, string? markdown = null)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var sessionKey = await db.PracticalModuleSessions.Where(s => s.Id == sessionId)
            .Select(s => s.SessionKey).SingleAsync();
        var handler = scope.ServiceProvider.GetRequiredService<IPracticeEventHandler>();
        await handler.HandleAsync(new PracticeEventInput(
            sessionId, sessionKey, Guid.NewGuid(), kind, DateTimeOffset.UtcNow, payloadJson, markdown));
    }

    // ---- helpers ----

    private sealed record BoundContext(Guid PracticalId, Guid TaskId, string ExternalTaskRef);

    private async Task<BoundContext> BindFreshExternalPracticalAsync(
        int triesCount, int? timeLimitMinutes, bool assignStudent = false)
    {
        // отдельная практика на каждый тест — свой независимый счётчик попыток
        var teacher = factory.CreateClient();
        teacher.AuthenticateAs(EducationRoles.Teacher);
        var practicalResponse = await teacher.PostAsJsonAsync(
            '/' + ApiRoutes.Practicals.PracticalsList,
            new CreatePracticalRequest(factory.Seed.OwnModuleId, "external practical " + Guid.NewGuid().ToString("N")[..8]));
        practicalResponse.EnsureSuccessStatusCode();
        var practicalId = (await practicalResponse.Content.ReadFromJsonAsync<PracticalResponse>())!.Id;

        if (assignStudent)
        {
            // назначает преподаватель-владелец практики (эндпоинт теперь TeacherOnly + владелец)
            var assign = await teacher.PutAsJsonAsync(
                '/' + ApiRoutes.Practicals.ForStudents(practicalId),
                new UpdatePracticalStudentsRequest([factory.Seed.TestUserId]));
            assign.EnsureSuccessStatusCode();
        }

        return await BindExistingPracticalAsync(practicalId, triesCount, timeLimitMinutes);
    }

    private async Task<BoundContext> BindExistingPracticalAsync(
        Guid practicalId, int triesCount, int? timeLimitMinutes = null)
    {
        var moduleId = await factory.RegisterModuleAsync();

        var teacher = factory.CreateClient();
        teacher.AuthenticateAs(EducationRoles.Teacher);
        const string externalRef = "sql-join-001";
        var bind = await teacher.PutAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModule(practicalId),
            new BindPracticalModuleRequest(moduleId, externalRef, triesCount, timeLimitMinutes));
        bind.EnsureSuccessStatusCode();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var taskId = await db.Cases
            .Where(c => c.PracticalMaterialId == practicalId && c.PracticalModuleId != null)
            .Select(c => c.Id)
            .SingleAsync();

        return new BoundContext(practicalId, taskId, externalRef);
    }

    private HttpClient StudentClient(IModulePushClient push) => ClientWith(push, EducationRoles.Student);

    private HttpClient ClientWith(IModulePushClient push, string role)
    {
        var client = factory.WithModuleFakes(push).CreateClient();
        client.AuthenticateAs(role);
        return client;
    }

    private static async Task<StartModuleSessionResponse> StartAsync(HttpClient client, BoundContext context)
    {
        var response = await client.PostAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForModuleSessions(context.PracticalId),
            new StartModuleSessionRequest(context.TaskId));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<StartModuleSessionResponse>())!;
    }

    private async Task<string> ReadSessionKeyAsync(Guid sessionId)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        return await db.PracticalModuleSessions.Where(s => s.Id == sessionId)
            .Select(s => s.SessionKey).SingleAsync();
    }

    private async Task<HttpResponseMessage> SendCompleteAsync(
        Guid sessionId, string serviceKey, string sessionKey, int grade, object? completionData = null)
    {
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(
            HttpMethod.Post, '/' + ApiRoutes.PracticalModules.ForSessionComplete(sessionId))
        {
            Content = JsonContent.Create(new
            {
                sessionKey,
                grade,
                completionData,
                completedAt = DateTimeOffset.UtcNow,
            }),
        };
        request.Headers.Add("X-Service-Key", serviceKey);
        return await client.SendAsync(request);
    }
}
