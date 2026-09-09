namespace Education.Kafka.Producing;

/// <summary>Отправка сообщения в топик, JSON, партиционирование по <paramref name="key"/>.</summary>
public interface IKafkaProducer<in TMessage>
{
    Task ProduceAsync(string topic, string key, TMessage message, CancellationToken ct = default);
}
