namespace Education.Application.Practicals.TaskAssignments;

/// <summary>
/// Формирование персональных случайных наборов заданий модуля для студентов
/// по правилам выборки практик курса.
/// </summary>
public interface ITaskAssignmentService
{
    /// <summary>
    /// Сформировать наборы для всей аудитории пары по всем практикам курса с правилом выборки.
    /// Студенты с уже существующим набором пропускаются. Недоступность каталога модуля
    /// не приводит к ошибке: генерация по этой практике откладывается до ленивой догенерации.
    /// </summary>
    /// <param name="classWindowId">Идентификатор открытой пары.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task GenerateForWindowAsync(Guid classWindowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Гарантировать набор студента по практике на пару: если набора ещё нет
    /// (поздний вход, точечное добавление, сбой при открытии пары) — сформировать.
    /// </summary>
    /// <param name="practicalId">Идентификатор практики.</param>
    /// <param name="classWindowId">Идентификатор пары.</param>
    /// <param name="studentId">Идентификатор студента.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task EnsureForStudentAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Пересобрать набор студента (только если по нему ещё не было ни одной сессии модуля).
    /// </summary>
    /// <param name="practicalId">Идентификатор практики.</param>
    /// <param name="classWindowId">Идентификатор пары.</param>
    /// <param name="studentId">Идентификатор студента.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task RegenerateForStudentAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default);
}
