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

    /// <summary>Последняя (по <c>StartedAt</c>) попытка пользователя по назначению случайного набора, отслеживаемая.</summary>
    Task<PracticalModuleSession?> GetLatestForUserAssignmentAsync(
        Guid userId,
        Guid assignmentId,
        CancellationToken cancellationToken = default);

    /// <summary>Всего попыток пользователя по назначению случайного набора, включая <c>EXPIRED</c>.</summary>
    Task<int> CountForUserAssignmentAsync(Guid userId, Guid assignmentId, CancellationToken cancellationToken = default);

    /// <summary>Максимальная оценка по <c>COMPLETED</c>-попыткам пользователя по назначению случайного набора.</summary>
    Task<int?> BestGradeForUserAssignmentAsync(Guid userId, Guid assignmentId, CancellationToken cancellationToken = default);

    /// <summary>Последние попытки по перечисленным назначениям случайных наборов (по одной на назначение).</summary>
    Task<IReadOnlyList<AssignmentSessionInfo>> ListForAssignmentsAsync(
        IReadOnlyCollection<Guid> assignmentIds,
        CancellationToken cancellationToken = default);

    /// <summary>Все попытки по практике (по всем заданиям и студентам), новые — первыми.</summary>
    Task<IReadOnlyList<ModuleSessionSummary>> ListForPracticalAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default);

    /// <summary>Есть ли активные (<c>ACTIVE</c>) сессии по практике — по заданиям и по назначениям случайных наборов.</summary>
    Task<bool> HasActiveForPracticalAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>Есть ли активные сессии по любой практике модуля.</summary>
    Task<bool> HasActiveForModuleAsync(Guid moduleId, CancellationToken cancellationToken = default);

    /// <summary>Есть ли активные сессии по любой практике курса.</summary>
    Task<bool> HasActiveForCourseAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>Добавляет новую сессию и сохраняет изменения.</summary>
    /// <param name="session">Новая сессия.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task AddAsync(PracticalModuleSession session, CancellationToken cancellationToken = default);

    /// <summary>Сохраняет изменения отслеживаемых сессий.</summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
