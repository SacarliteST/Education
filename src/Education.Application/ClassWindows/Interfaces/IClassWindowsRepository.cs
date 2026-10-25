using Education.Domain.Courses;

namespace Education.Application.ClassWindows;

/// <summary>Хранилище пар и запросы, нужные для проверки доступа к практике.</summary>
public interface IClassWindowsRepository
{
    /// <summary>Курс и режим по практике или <see langword="null"/>.</summary>
    Task<PracticeGate?> GetGateForPracticalAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>Курс и режим по заданию практики или <see langword="null"/>.</summary>
    Task<PracticeGate?> GetGateForTaskAsync(Guid taskId, CancellationToken cancellationToken = default);

    /// <summary>Курс (отслеживаемый) или <see langword="null"/>.</summary>
    Task<Course?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>Пара курса и группы, идущая в момент <paramref name="at"/> (с отсрочкой окончания <paramref name="grace"/>).</summary>
    Task<ClassWindow?> FindOpenAsync(
        Guid courseId, string groupName, DateTimeOffset at, TimeSpan grace, CancellationToken cancellationToken = default);

    /// <summary>Незавершённая вручную пара курса и группы (отслеживаемая).</summary>
    Task<ClassWindow?> GetUnclosedAsync(Guid courseId, string groupName, CancellationToken cancellationToken = default);

    /// <summary>Пара по идентификатору (отслеживаемая).</summary>
    Task<ClassWindow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Последние пары курса, новые — первыми.</summary>
    Task<IReadOnlyList<ClassWindow>> ListForCourseAsync(Guid courseId, int take, CancellationToken cancellationToken = default);

    /// <summary>Группы студентов, назначенных на курс.</summary>
    Task<IReadOnlyList<string>> ListGroupsAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>Идущие сессии внешних модулей студентов группы по курсу (отслеживаемые).</summary>
    Task<IReadOnlyList<ActiveSessionTarget>> ListActiveSessionsAsync(
        Guid courseId, string groupName, CancellationToken cancellationToken = default);

    /// <summary>Идущие сессии студентов с группой, отличной от нужной не учитываются; все идущие сессии курса.</summary>
    Task AddAsync(ClassWindow window, CancellationToken cancellationToken = default);

    /// <summary>Сохраняет изменения.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
