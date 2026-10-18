using System.Text.Json;
using Education.Contracts.Kafka;


namespace Education.Tests.PracticalModules;

public sealed class PracticeEventMessageJsonTests
{
    // Те же опции, что на проводе: Education.Kafka.KafkaJson (internal) = JsonSerializerDefaults.Web.
    private static readonly JsonSerializerOptions WireOptions = new(JsonSerializerDefaults.Web);

    private const string Base =
        "\"sessionId\":\"11111111-1111-1111-1111-111111111111\",\"sessionKey\":\"k\"," +
        "\"eventId\":\"22222222-2222-2222-2222-222222222222\",\"kind\":\"sql_submit\"," +
        "\"occurredAt\":\"2026-10-18T12:00:00+00:00\",\"payload\":{\"isCorrect\":true}";

    [Fact]
    public void OldMessageWithoutMarkdown_Deserializes_MarkdownNull()
    {
        var message = JsonSerializer.Deserialize<PracticeEventMessage>("{" + Base + "}", WireOptions);

        Assert.NotNull(message);
        Assert.Null(message!.Markdown);
        Assert.Equal("sql_submit", message.Kind);
        Assert.True(message.Payload.GetProperty("isCorrect").GetBoolean());
    }

    [Fact]
    public void MessageWithMarkdown_Deserializes_MarkdownKept()
    {
        var json = "{" + Base + ",\"markdown\":\"**Попытка 1 — верно**\"}";

        var message = JsonSerializer.Deserialize<PracticeEventMessage>(json, WireOptions);

        Assert.Equal("**Попытка 1 — верно**", message!.Markdown);
    }

    [Fact]
    public void MessageWithNullMarkdown_Deserializes_MarkdownNull()
    {
        var message = JsonSerializer.Deserialize<PracticeEventMessage>("{" + Base + ",\"markdown\":null}", WireOptions);

        Assert.Null(message!.Markdown);
    }

    [Fact]
    public void MessageWithUnknownFutureField_StillDeserializes()
    {
        var message = JsonSerializer.Deserialize<PracticeEventMessage>("{" + Base + ",\"markdownVersion\":2}", WireOptions);

        Assert.NotNull(message);
    }
}
