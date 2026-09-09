using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Education.Kafka.Consuming;

/// <summary>
/// Базовый фоновый консьюмер одного топика. Коммитит офсет только после успешного
/// <see cref="HandleAsync"/> — упавший хендлер переигрывается при рестарте, обработчик
/// должен быть идемпотентным.
/// </summary>
public abstract class KafkaConsumerBackgroundService<TMessage> : BackgroundService
{
    private readonly IConsumer<string, string> consumer;
    private readonly ILogger logger;

    protected KafkaConsumerBackgroundService(IOptions<KafkaOptions> options, ILogger logger)
    {
        this.logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = options.Value.ConsumerGroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
        consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected abstract string Topic { get; }

    protected abstract Task HandleAsync(TMessage message, CancellationToken ct);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        consumer.Subscribe(Topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ConsumeResult<string, string>? result;
                try
                {
                    result = consumer.Consume(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException ex)
                {
                    logger.LogError(ex, "Ошибка чтения топика {Topic}", Topic);
                    continue;
                }

                if (result?.Message is null)
                {
                    continue;
                }

                await ProcessAsync(result, stoppingToken);
            }
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task ProcessAsync(ConsumeResult<string, string> result, CancellationToken ct)
    {
        TMessage? message;
        try
        {
            message = JsonSerializer.Deserialize<TMessage>(result.Message.Value, KafkaJson.Options);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Не удалось десериализовать сообщение {Topic}, offset {Offset}", Topic, result.Offset);
            consumer.Commit(result);
            return;
        }

        if (message is null)
        {
            consumer.Commit(result);
            return;
        }

        try
        {
            await HandleAsync(message, ct);
            consumer.Commit(result);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Ошибка обработки сообщения {Topic}, offset {Offset}", Topic, result.Offset);
        }
    }

    public override void Dispose()
    {
        consumer.Dispose();
        base.Dispose();
    }
}
