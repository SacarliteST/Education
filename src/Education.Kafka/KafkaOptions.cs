namespace Education.Kafka;

/// <summary>Настройки подключения к брокеру. Секция конфигурации — <c>Kafka</c>.</summary>
public sealed class KafkaOptions
{
    public const string SectionKey = "Kafka";

    public string BootstrapServers { get; init; } = String.Empty;
    public string ConsumerGroupId { get; init; } = "education";
}
