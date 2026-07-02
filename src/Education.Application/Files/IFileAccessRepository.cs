namespace Education.Application.Files;

/// <summary>
/// Проверяет доступ текущего пользователя к метаданным сохранённых файлов.
/// </summary>
public interface IFileAccessRepository
{
    /// <summary>
    /// Возвращает файл, доступный студенту как владельцу сдачи.
    /// </summary>
    Task<StoredFileInfo?> GetStudentTaskFileAsync(
        string storageKey,
        long studentUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает файл сдачи, доступный преподавателю своего курса.
    /// </summary>
    Task<StoredFileInfo?> GetTeacherTaskFileAsync(
        string storageKey,
        long teacherUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает документ теории, доступный студенту назначенного курса.
    /// </summary>
    Task<StoredFileInfo?> GetStudentTheoryDocumentAsync(
        string storageKey,
        long studentUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает документ теории, доступный преподавателю своего курса.
    /// </summary>
    Task<StoredFileInfo?> GetTeacherTheoryDocumentAsync(
        string storageKey,
        long teacherUserId,
        CancellationToken cancellationToken = default);
}
