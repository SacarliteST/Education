using Education.Domain.Practicals;

namespace Education.Application.Practicals.TaskAssignments;

/// <summary>Хранилище материализованных наборов заданий модуля, выданных студентам на пару.</summary>
public interface IPracticalTaskAssignmentsRepository
{
    /// <summary>Назначение по идентификатору или <see langword="null"/>.</summary>
    Task<PracticalTaskAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Назначения практики на пару (для таблицы преподавателя).</summary>
    Task<IReadOnlyList<PracticalTaskAssignment>> ListAsync(
        Guid practicalId, Guid classWindowId, CancellationToken cancellationToken = default);

    /// <summary>Назначения студента по практике на пару (по порядку набора).</summary>
    Task<IReadOnlyList<PracticalTaskAssignment>> ListForStudentAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default);

    /// <summary>Есть ли у студента уже сформированный набор по практике на пару.</summary>
    Task<bool> ExistsForStudentAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default);

    /// <summary>Есть ли по назначениям студента на пару хотя бы одна сессия модуля.</summary>
    Task<bool> HasSessionsForStudentAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default);

    /// <summary>Есть ли по всем назначениям практики хотя бы одна сессия модуля.</summary>
    Task<bool> HasAnySessionsForPracticalAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>Последняя по времени начала пара, на которую студенту сформирован набор по практике.</summary>
    Task<Guid?> FindLatestWindowIdForStudentAsync(
        Guid practicalId, Guid studentId, CancellationToken cancellationToken = default);

    /// <summary>Добавить набор назначений.</summary>
    Task AddRangeAsync(IEnumerable<PracticalTaskAssignment> assignments, CancellationToken cancellationToken = default);

    /// <summary>Удалить набор студента по практике на пару (пересбор).</summary>
    Task DeleteForStudentAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default);

    /// <summary>Сохранить изменения.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
