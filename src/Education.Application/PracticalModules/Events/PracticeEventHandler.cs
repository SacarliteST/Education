using Education.Domain.PracticalModules;
using Microsoft.Extensions.Logging;

namespace Education.Application.PracticalModules;

/// <summary>Обработка событий «цифрового следа» практики, присланных внешним модулем.</summary>
public sealed class PracticeEventHandler : IPracticeEventHandler
{
    private readonly IPracticalModuleSessionsRepository sessionsRepository;
    private readonly IPracticalTaskEventsRepository eventsRepository;
    private readonly ILogger<PracticeEventHandler> logger;

    /// <summary>Создаёт экземпляр класса <see cref="PracticeEventHandler"/>.</summary>
    /// <param name="sessionsRepository">Хранилище сессий внешних модулей.</param>
    /// <param name="eventsRepository">Хранилище событий заданий практики.</param>
    /// <param name="logger">Журнал.</param>
    public PracticeEventHandler(
        IPracticalModuleSessionsRepository sessionsRepository,
        IPracticalTaskEventsRepository eventsRepository,
        ILogger<PracticeEventHandler> logger)
    {
        this.sessionsRepository = sessionsRepository;
        this.eventsRepository = eventsRepository;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task HandleAsync(PracticeEventInput input, CancellationToken cancellationToken = default)
    {
        var session = await sessionsRepository.GetByIdAsync(input.SessionId, cancellationToken);

        // Неизвестная сессия или неверный секрет — молча отбрасываем (не наш продюсер).
        if (session is null || session.SessionKey != input.SessionKey)
        {
            logger.LogWarning(
                "Событие {EventId} по сессии {SessionId} отброшено: сессия не найдена или sessionKey не совпадает.",
                input.EventId, input.SessionId);
            return;
        }

        // Состояние сессии не проверяем: Kafka асинхронна, и событие «победной» попытки
        // почти всегда приходит уже ПОСЛЕ HTTP-оценки, когда сессия COMPLETED. Журнал —
        // append-only; границы задают проверка sessionKey и дедуп по eventId, а не статус.

        await eventsRepository.InsertIfNewAsync(
            new PracticalTaskEvent(
                input.EventId, input.SessionId, input.Kind, input.OccurredAt, input.PayloadJson, input.Markdown),
            cancellationToken);
        logger.LogDebug(
            "Событие {EventId} ({Kind}) добавлено в цифровой след сессии {SessionId}.",
            input.EventId, input.Kind, input.SessionId);
    }
}
