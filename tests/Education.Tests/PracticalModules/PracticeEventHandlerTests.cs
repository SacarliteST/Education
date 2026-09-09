using Education.Application.PracticalModules;
using Education.Domain.PracticalModules;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Tests.PracticalModules;

public sealed class PracticeEventHandlerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public PracticeEventHandlerTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task ActiveSession_ValidKey_EventStored()
    {
        var sessionId = await SeedSessionAsync("key-1", "ACTIVE");

        await HandleAsync(new PracticeEventInput(
            sessionId, "key-1", Guid.NewGuid(), "sql_submit", DateTimeOffset.UtcNow, "{\"sql\":\"SELECT 1\"}"));

        var events = await ReadEventsAsync(sessionId);
        Assert.Single(events);
        Assert.Equal("sql_submit", events[0].Kind);
    }

    [Fact]
    public async Task WrongSessionKey_Ignored()
    {
        var sessionId = await SeedSessionAsync("key-2", "ACTIVE");

        await HandleAsync(new PracticeEventInput(
            sessionId, "not-key-2", Guid.NewGuid(), "sql_submit", DateTimeOffset.UtcNow, "{}"));

        Assert.Empty(await ReadEventsAsync(sessionId));
    }

    [Fact]
    public async Task UnknownSession_Ignored()
    {
        await HandleAsync(new PracticeEventInput(
            Guid.NewGuid(), "whatever", Guid.NewGuid(), "sql_submit", DateTimeOffset.UtcNow, "{}"));
        // не бросает исключение — достаточно
    }

    [Fact]
    public async Task CompletedSession_ValidKey_EventStored()
    {
        // Событие «победной» попытки приходит из Kafka уже после HTTP-оценки,
        // когда сессия COMPLETED — оно всё равно должно попасть в журнал.
        var sessionId = await SeedSessionAsync("key-3", "COMPLETED");

        await HandleAsync(new PracticeEventInput(
            sessionId, "key-3", Guid.NewGuid(), "sql_submit", DateTimeOffset.UtcNow, "{\"isCorrect\":true}"));

        Assert.Single(await ReadEventsAsync(sessionId));
    }

    [Fact]
    public async Task ExpiredSession_ValidKey_EventStored()
    {
        // Журнал append-only; поздние события прерванной/просроченной попытки — тоже след.
        var sessionId = await SeedSessionAsync("key-3b", "EXPIRED");

        await HandleAsync(new PracticeEventInput(
            sessionId, "key-3b", Guid.NewGuid(), "sql_submit", DateTimeOffset.UtcNow, "{}"));

        Assert.Single(await ReadEventsAsync(sessionId));
    }

    [Fact]
    public async Task DuplicateEventId_NotDuplicated()
    {
        var sessionId = await SeedSessionAsync("key-4", "ACTIVE");
        var eventId = Guid.NewGuid();

        await HandleAsync(new PracticeEventInput(sessionId, "key-4", eventId, "sql_submit", DateTimeOffset.UtcNow, "{}"));
        await HandleAsync(new PracticeEventInput(sessionId, "key-4", eventId, "sql_submit", DateTimeOffset.UtcNow, "{}"));

        Assert.Single(await ReadEventsAsync(sessionId));
    }

    private async Task<Guid> SeedSessionAsync(string sessionKey, string status)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var session = new PracticalModuleSession(
            Guid.NewGuid(),
            factory.Seed.AssignedTaskId,
            factory.Seed.TestUserId,
            1,
            sessionKey,
            "http://platform/return",
            DateTimeOffset.UtcNow,
            null);
        if (status == "EXPIRED")
        {
            session.Expire(DateTimeOffset.UtcNow, ModuleSessionEndReason.Timeout);
        }
        else if (status == "COMPLETED")
        {
            session.Complete(DateTimeOffset.UtcNow, 100, "{}");
        }

        db.PracticalModuleSessions.Add(session);
        await db.SaveChangesAsync();
        return session.Id;
    }

    private async Task HandleAsync(PracticeEventInput input)
    {
        using var scope = factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IPracticeEventHandler>();
        await handler.HandleAsync(input);
    }

    private async Task<IReadOnlyList<PracticalTaskEvent>> ReadEventsAsync(Guid sessionId)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        return await db.PracticalTaskEvents.AsNoTracking()
            .Where(taskEvent => taskEvent.SessionId == sessionId)
            .ToListAsync();
    }
}
