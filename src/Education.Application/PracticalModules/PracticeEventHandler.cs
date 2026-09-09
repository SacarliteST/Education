using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

public sealed class PracticeEventHandler(
    IPracticalModuleSessionsRepository sessionsRepository,
    IPracticalTaskEventsRepository eventsRepository) : IPracticeEventHandler
{
    public async Task HandleAsync(PracticeEventInput input, CancellationToken cancellationToken = default)
    {
        var session = await sessionsRepository.GetByIdAsync(input.SessionId, cancellationToken);

        // Неизвестная сессия или неверный секрет — молча отбрасываем (не наш продюсер).
        if (session is null || session.SessionKey != input.SessionKey)
        {
            return;
        }

        // Состояние сессии не проверяем: Kafka асинхронна, и событие «победной» попытки
        // почти всегда приходит уже ПОСЛЕ HTTP-оценки, когда сессия COMPLETED. Журнал —
        // append-only; границы задают проверка sessionKey и дедуп по eventId, а не статус.

        await eventsRepository.InsertIfNewAsync(
            new PracticalTaskEvent(
                input.EventId, input.SessionId, input.Kind, input.OccurredAt, input.PayloadJson),
            cancellationToken);
    }
}
