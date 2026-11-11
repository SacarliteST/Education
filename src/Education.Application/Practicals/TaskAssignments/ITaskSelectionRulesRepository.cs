using Education.Domain.Practicals;

namespace Education.Application.Practicals.TaskAssignments;

/// <summary>Хранилище правил случайной выборки заданий модуля для практик.</summary>
public interface ITaskSelectionRulesRepository
{
    /// <summary>Правило практики (отслеживаемое) или <see langword="null"/>.</summary>
    Task<TaskSelectionRule?> GetByPracticalIdAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>Правила всех практик курса, у которых настроена случайная выборка.</summary>
    Task<IReadOnlyList<TaskSelectionRule>> ListForCourseAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>Добавить правило.</summary>
    Task AddAsync(TaskSelectionRule rule, CancellationToken cancellationToken = default);

    /// <summary>Удалить правило.</summary>
    void Remove(TaskSelectionRule rule);

    /// <summary>Сохранить изменения.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
