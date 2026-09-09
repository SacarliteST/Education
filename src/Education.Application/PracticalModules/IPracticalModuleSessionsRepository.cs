using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

/// <summary>Хранилище попыток прохождения внешних практических модулей.</summary>
public interface IPracticalModuleSessionsRepository
{
    /// <summary>Отслеживаемая сущность или <see langword="null"/>.</summary>
    Task<PracticalModuleSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Последняя (по <c>StartedAt</c>) попытка пользователя по заданию, отслеживаемая.</summary>
    Task<PracticalModuleSession?> GetLatestForUserTaskAsync(
        Guid userId,
        Guid practicalTaskId,
        CancellationToken cancellationToken = default);

    /// <summary>Всего попыток пользователя по заданию, включая <c>EXPIRED</c>.</summary>
    Task<int> CountForUserTaskAsync(Guid userId, Guid practicalTaskId, CancellationToken cancellationToken = default);

    /// <summary>Максимальная оценка по <c>COMPLETED</c>-попыткам пользователя по заданию.</summary>
    Task<int?> BestGradeForUserTaskAsync(Guid userId, Guid practicalTaskId, CancellationToken cancellationToken = default);

    /// <summary>Все попытки по практике (по всем заданиям и студентам), новые — первыми.</summary>
    Task<IReadOnlyList<ModuleSessionSummary>> ListForPracticalAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default);

    Task AddAsync(PracticalModuleSession session, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
