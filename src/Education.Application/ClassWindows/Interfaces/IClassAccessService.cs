namespace Education.Application.ClassWindows;

/// <summary>Проверка доступа студента к тестам и практике курса с учётом пар.</summary>
public interface IClassAccessService
{
    /// <summary>Конец пары, ограничивающий внешнюю сессию, или <see langword="null"/> (режим выключен).</summary>
    /// <exception cref="ClassWindowClosedException">Режим включён, пары нет.</exception>
    Task<DateTimeOffset?> RequireWindowEndAsync(Guid courseId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Проверка для теста практики. <paramref name="withGrace"/> — принять сдачу чуть после конца пары.</summary>
    Task EnsureOpenForPracticalAsync(
        Guid practicalId, Guid userId, bool withGrace = false, CancellationToken cancellationToken = default);

    /// <summary>Проверка для сдачи файла по заданию.</summary>
    Task EnsureOpenForTaskAsync(Guid taskId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Состояние доступа студента к курсу.</summary>
    Task<ClassAccessState> GetStateAsync(Guid courseId, Guid userId, CancellationToken cancellationToken = default);
}
