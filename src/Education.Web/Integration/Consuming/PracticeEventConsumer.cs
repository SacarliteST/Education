using Education.Application.PracticalModules;
using Education.Contracts.Kafka;
using Education.Kafka;
using Education.Kafka.Consuming;
using Microsoft.Extensions.Options;

namespace Education.Web.Integration;

/// <summary>
/// Консьюмер топика <see cref="KafkaTopics.PracticeEvents"/>. Только пополняет ленту
/// «цифрового следа»; терминальных переходов сессии здесь нет (оценка приходит по HTTP).
/// </summary>
internal sealed class PracticeEventConsumer : KafkaConsumerBackgroundService<PracticeEventMessage>
{
    private readonly IServiceScopeFactory scopeFactory;

    /// <summary>Создаёт экземпляр класса <see cref="PracticeEventConsumer"/>.</summary>
    /// <param name="options">Настройки подключения к Kafka.</param>
    /// <param name="logger">Журнал.</param>
    /// <param name="scopeFactory">Фабрика областей видимости для обработчика события.</param>
    public PracticeEventConsumer(
        IOptions<KafkaOptions> options,
        ILogger<PracticeEventConsumer> logger,
        IServiceScopeFactory scopeFactory)
        : base(options, logger)
    {
        this.scopeFactory = scopeFactory;
    }

    /// <inheritdoc />
    protected override string Topic => KafkaTopics.PracticeEvents;

    /// <inheritdoc />
    protected override async Task HandleAsync(PracticeEventMessage message, CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<IPracticeEventHandler>();

        await handler.HandleAsync(
            new PracticeEventInput(
                message.SessionId,
                message.SessionKey,
                message.EventId,
                message.Kind,
                message.OccurredAt,
                message.Payload.GetRawText(),
                message.Markdown),
            ct);
    }
}
