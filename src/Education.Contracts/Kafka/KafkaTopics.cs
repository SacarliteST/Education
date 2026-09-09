namespace Education.Contracts.Kafka;

/// <summary>Топики подключения практических модулей.</summary>
public static class KafkaTopics
{
    /// <summary>«Цифровой след» — журнал действий студента в модуле. Оценка идёт по HTTP, не через Kafka.</summary>
    public const string PracticeEvents = "scoodle.practice.events";
}
