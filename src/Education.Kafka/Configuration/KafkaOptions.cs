namespace Education.Kafka;

/// <summary>Настройки подключения к брокеру. Секция конфигурации — <c>Kafka</c>.</summary>
public sealed class KafkaOptions
{
    /// <summary>Имя секции конфигурации.</summary>
    public const string SectionKey = "Kafka";

    /// <summary>Адреса брокеров (bootstrap servers).</summary>
    public string BootstrapServers { get; init; } = String.Empty;
    /// <summary>Идентификатор группы консьюмеров.</summary>
    public string ConsumerGroupId { get; init; } = "education";
}
