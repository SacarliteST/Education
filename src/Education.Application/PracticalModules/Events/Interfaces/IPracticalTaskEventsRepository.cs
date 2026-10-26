using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

/// <summary>Хранилище журнала действий («цифрового следа»).</summary>
public interface IPracticalTaskEventsRepository
{
    /// <summary>Вставляет событие, если события с таким <c>Id</c> ещё нет (идемпотентность переигрывания Kafka).</summary>
    Task InsertIfNewAsync(PracticalTaskEvent taskEvent, CancellationToken cancellationToken = default);

    /// <summary>Сколько событий заданного вида записано по сессии.</summary>
    /// <param name="sessionId">Идентификатор сессии.</param>
    /// <param name="kind">Вид события (например, <c>sql_submit</c>).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<int> CountByKindAsync(Guid sessionId, string kind, CancellationToken cancellationToken = default);

    /// <summary>События сессии в порядке возникновения.</summary>
    Task<IReadOnlyList<PracticalTaskEvent>> GetForSessionAsync(Guid sessionId, CancellationToken cancellationToken = default);
}
