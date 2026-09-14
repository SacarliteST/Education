using Education.Domain.PracticalModules;
using Microsoft.Extensions.Logging;

namespace Education.Application.PracticalModules;

public sealed class PracticeEventHandler(
    IPracticalModuleSessionsRepository sessionsRepository,
    IPracticalTaskEventsRepository eventsRepository,
    ILogger<PracticeEventHandler> logger) : IPracticeEventHandler
{
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
                input.EventId, input.SessionId, input.Kind, input.OccurredAt, input.PayloadJson),
            cancellationToken);
        logger.LogDebug(
            "Событие {EventId} ({Kind}) добавлено в цифровой след сессии {SessionId}.",
            input.EventId, input.Kind, input.SessionId);
    }
}
