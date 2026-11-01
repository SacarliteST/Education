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

    /// <summary>Пара курса с точечным доступом студента, идущая в момент <paramref name="at"/> (с отсрочкой <paramref name="grace"/>).</summary>
    Task<ClassWindow?> FindOpenForUserAsync(
        Guid courseId, Guid userId, DateTimeOffset at, TimeSpan grace, CancellationToken cancellationToken = default);

    /// <summary>Незавершённые вручную пары курса со студентами (отслеживаемые).</summary>
    Task<IReadOnlyList<ClassWindow>> ListUnclosedAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>Пара по идентификатору со студентами (отслеживаемая).</summary>
    Task<ClassWindow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Последние пары курса со студентами, новые — первыми.</summary>
    Task<IReadOnlyList<ClassWindow>> ListForCourseAsync(Guid courseId, int take, CancellationToken cancellationToken = default);

    /// <summary>Группы студентов, назначенных на курс.</summary>
    Task<IReadOnlyList<string>> ListGroupsAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>Студенты, назначенные на курс (для выбора аудитории пары).</summary>
    Task<IReadOnlyList<ClassWindowStudentOption>> ListStudentsAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>Идущие сессии внешних модулей студентов группы и/или перечисленных студентов по курсу (отслеживаемые).</summary>
    Task<IReadOnlyList<ActiveSessionTarget>> ListActiveSessionsAsync(
        Guid courseId, string? groupName, IReadOnlyCollection<Guid> studentUserIds, CancellationToken cancellationToken = default);

    /// <summary>Добавляет пару.</summary>
    Task AddAsync(ClassWindow window, CancellationToken cancellationToken = default);

    /// <summary>Сохраняет изменения.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
