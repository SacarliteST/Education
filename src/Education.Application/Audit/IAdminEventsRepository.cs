using Education.Domain.Audit;

namespace Education.Application.Audit;

/// <summary>Чтение журнала административных действий Education.</summary>
public interface IAdminEventsRepository
{
    /// <summary>Возвращает последние события, от новых к старым.</summary>
    Task<IReadOnlyList<AdminEvent>> GetRecentAsync(int take, CancellationToken cancellationToken = default);
}
