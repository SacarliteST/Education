using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

/// <summary>Хранилище журнала действий («цифрового следа»).</summary>
public interface IPracticalTaskEventsRepository
{
    /// <summary>Вставляет событие, если события с таким <c>Id</c> ещё нет (идемпотентность переигрывания Kafka).</summary>
    Task InsertIfNewAsync(PracticalTaskEvent taskEvent, CancellationToken cancellationToken = default);

    /// <summary>События сессии в порядке возникновения.</summary>
    Task<IReadOnlyList<PracticalTaskEvent>> GetForSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
}
