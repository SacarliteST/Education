namespace Education.Application.ClassWindows;

/// <summary>Управление парами курса преподавателем.</summary>
public interface IClassWindowsService
{
    /// <summary>Режим курса, группы и пары.</summary>
    Task<CourseClassWindows> GetAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>Включает или выключает режим «практика только на парах».</summary>
    Task SetModeAsync(Guid courseId, bool practiceOnlyDuringClass, CancellationToken cancellationToken = default);

    /// <summary>Начинает пару для группы.</summary>
    Task<ClassWindowInfo> StartAsync(
        Guid courseId, string groupName, int durationMinutes, CancellationToken cancellationToken = default);

    /// <summary>Продлевает идущую пару.</summary>
    Task<ClassWindowInfo> ExtendAsync(Guid windowId, int minutes, CancellationToken cancellationToken = default);

    /// <summary>Завершает пару досрочно.</summary>
    Task<ClassWindowInfo> FinishAsync(Guid windowId, CancellationToken cancellationToken = default);
}
