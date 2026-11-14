namespace Education.Application.ClassWindows;

/// <summary>
/// Определяет временные границы пары для выборки учебных результатов.
/// </summary>
public interface IClassWindowResultsScopeResolver
{
    /// <summary>
    /// Возвращает границы выбранной пары или <see langword="null"/> для выборки за всё время.
    /// </summary>
    /// <param name="courseId">Курс, в рамках которого запрашиваются результаты.</param>
    /// <param name="classWindowId">Выбранная пара или <see langword="null"/>.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <exception cref="ClassWindowNotFoundException">Пара не найдена.</exception>
    /// <exception cref="ClassWindowCourseMismatchException">Пара относится к другому курсу.</exception>
    Task<ClassWindowResultsScope?> ResolveAsync(
        Guid courseId,
        Guid? classWindowId,
        CancellationToken cancellationToken = default);
}
