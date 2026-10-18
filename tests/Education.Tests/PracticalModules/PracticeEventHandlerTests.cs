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

    [Fact]
    public async Task EventWithMarkdown_MarkdownStoredNextToPayload()
    {
        var sessionId = await SeedSessionAsync("key-md-1", "ACTIVE");
        const string markdown = "**Попытка 1 — верно**\n\n```sql\nSELECT 1\n```";

        await HandleAsync(new PracticeEventInput(
            sessionId, "key-md-1", Guid.NewGuid(), "sql_submit", DateTimeOffset.UtcNow, "{\"isCorrect\":true}", markdown));

        var stored = Assert.Single(await ReadEventsAsync(sessionId));
        Assert.Equal(markdown, stored.Markdown);
        Assert.Contains("isCorrect", stored.Payload);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   \n ")]
    public async Task EventWithoutMarkdown_StoredAsNull(string? markdown)
    {
        var sessionId = await SeedSessionAsync("key-md-2", "ACTIVE");

        await HandleAsync(new PracticeEventInput(
            sessionId, "key-md-2", Guid.NewGuid(), "sql_submit", DateTimeOffset.UtcNow, "{}", markdown));

        Assert.Null(Assert.Single(await ReadEventsAsync(sessionId)).Markdown);
    }

    [Fact]
    public async Task OversizedMarkdown_TruncatedWithMarker_EventNotDropped()
    {
        var sessionId = await SeedSessionAsync("key-md-3", "ACTIVE");
        // 40 000 русских букв = 80 000 байт UTF-8 — больше лимита 64 KiB.
        var huge = new string('ж', 40_000);

        await HandleAsync(new PracticeEventInput(
            sessionId, "key-md-3", Guid.NewGuid(), "sql_submit", DateTimeOffset.UtcNow, "{}", huge));

        var stored = Assert.Single(await ReadEventsAsync(sessionId)).Markdown;
        Assert.NotNull(stored);
        Assert.EndsWith(PracticalTaskEvent.MarkdownTruncationMarker, stored);
        Assert.True(System.Text.Encoding.UTF8.GetByteCount(stored) <= PracticalTaskEvent.MaxMarkdownBytes);
        Assert.True(stored.Length > 30_000);
    }

    [Fact]
    public async Task OversizedMarkdown_NotCutInsideSurrogatePair()
    {
        var sessionId = await SeedSessionAsync("key-md-4", "ACTIVE");
        // Эмодзи — суррогатная пара (4 байта UTF-8); граница лимита не должна её рвать.
        var huge = string.Concat(Enumerable.Repeat("😀", 20_000));

        await HandleAsync(new PracticeEventInput(
            sessionId, "key-md-4", Guid.NewGuid(), "sql_submit", DateTimeOffset.UtcNow, "{}", huge));

        var stored = Assert.Single(await ReadEventsAsync(sessionId)).Markdown!;
        var body = stored[..^PracticalTaskEvent.MarkdownTruncationMarker.Length];
        Assert.False(char.IsHighSurrogate(body[^1]));
        Assert.True(System.Text.Encoding.UTF8.GetByteCount(stored) <= PracticalTaskEvent.MaxMarkdownBytes);
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
