using System.Text.Json;

namespace Education.Kafka;

/// <summary>Общие опции сериализации сообщений — camelCase на проводе.</summary>
internal static class KafkaJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
}
