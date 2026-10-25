using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Education.Kafka.Producing;

/// <summary>Продюсер Kafka на базе Confluent.Kafka, сообщения передаются в виде JSON.</summary>
/// <typeparam name="TMessage">Тип сообщения.</typeparam>
internal sealed class KafkaProducer<TMessage> : IKafkaProducer<TMessage>, IDisposable
{
    private readonly IProducer<string, string> producer;

    /// <summary>Создаёт продюсера.</summary>
    /// <param name="options">Настройки подключения к брокеру.</param>
    public KafkaProducer(IOptions<KafkaOptions> options)
    {
        var config = new ProducerConfig { BootstrapServers = options.Value.BootstrapServers };
        producer = new ProducerBuilder<string, string>(config).Build();
    }

    /// <inheritdoc />
    public async Task ProduceAsync(string topic, string key, TMessage message, CancellationToken ct = default)
    {
        var value = JsonSerializer.Serialize(message, KafkaJson.Options);
        await producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = value }, ct);
    }

    /// <inheritdoc />
    public void Dispose() => producer.Dispose();
}
