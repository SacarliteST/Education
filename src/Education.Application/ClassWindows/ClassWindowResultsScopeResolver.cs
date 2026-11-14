namespace Education.Application.ClassWindows;

/// <summary>
/// Определяет границы пары по данным хранилища и проверяет её принадлежность курсу.
/// </summary>
public sealed class ClassWindowResultsScopeResolver : IClassWindowResultsScopeResolver
{
    private readonly IClassWindowsRepository repository;

    /// <summary>
    /// Создаёт resolver границ пары.
    /// </summary>
    /// <param name="repository">Хранилище пар.</param>
    public ClassWindowResultsScopeResolver(IClassWindowsRepository repository)
    {
        this.repository = repository;
    }

    /// <inheritdoc />
    public async Task<ClassWindowResultsScope?> ResolveAsync(
        Guid courseId,
        Guid? classWindowId,
        CancellationToken cancellationToken = default)
    {
        if (classWindowId is null)
        {
            return null;
        }

        var window = await repository.GetByIdAsync(classWindowId.Value, cancellationToken)
            ?? throw new ClassWindowNotFoundException();
        if (window.CourseId != courseId)
        {
            throw new ClassWindowCourseMismatchException();
        }

        return new ClassWindowResultsScope(window.Id, window.StartsAt, window.EffectiveEnd);
    }
}
