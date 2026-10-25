namespace Education.Kafka.Producing;

/// <summary>Отправка сообщения в топик: сериализация в JSON, партиционирование по ключу.</summary>
/// <typeparam name="TMessage">Тип сообщения.</typeparam>
public interface IKafkaProducer<in TMessage>
{
    /// <summary>Отправляет сообщение в топик.</summary>
    /// <param name="topic">Имя топика.</param>
    /// <param name="key">Ключ партиционирования.</param>
    /// <param name="message">Сообщение.</param>
    /// <param name="ct">Токен отмены.</param>
    Task ProduceAsync(string topic, string key, TMessage message, CancellationToken ct = default);
}
