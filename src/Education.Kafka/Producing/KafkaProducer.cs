using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Education.Kafka.Producing;

internal sealed class KafkaProducer<TMessage> : IKafkaProducer<TMessage>, IDisposable
{
    private readonly IProducer<string, string> producer;

    public KafkaProducer(IOptions<KafkaOptions> options)
    {
        var config = new ProducerConfig { BootstrapServers = options.Value.BootstrapServers };
        producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, string key, TMessage message, CancellationToken ct = default)
    {
        var value = JsonSerializer.Serialize(message, KafkaJson.Options);
        await producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = value }, ct);
    }

    public void Dispose() => producer.Dispose();
}
