using Education.Domain.Practicals;

namespace Education.Application.TaskFiles;

/// <summary>
/// Предоставляет операции чтения и записи файлов сдачи заданий.
/// </summary>
public interface ITaskFilesRepository
{
    /// <summary>
    /// Проверяет, назначено ли задание указанному студенту.
    /// </summary>
    Task<bool> IsTaskAssignedToStudentAsync(long taskId, long studentUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли задание курсу указанного преподавателя.
    /// </summary>
    Task<bool> IsTaskOwnedByTeacherAsync(long taskId, long teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли практический материал курсу указанного преподавателя.
    /// </summary>
    Task<bool> IsPracticalOwnedByTeacherAsync(long practicalId, long teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли файл сдачи курсу указанного преподавателя.
    /// </summary>
    Task<bool> IsTaskFileOwnedByTeacherAsync(long taskFileId, long teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает файл сдачи текущего студента по заданию.
    /// </summary>
    Task<CaseFile?> GetStudentTaskFileAsync(long taskId, long studentUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает файлы сдачи указанного задания для преподавателя.
    /// </summary>
    Task<IReadOnlyList<CaseFile>> GetTeacherTaskFilesAsync(long taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает файлы сдачи всех заданий практического материала для преподавателя.
    /// </summary>
    Task<IReadOnlyList<CaseFile>> GetTeacherPracticalTaskFilesAsync(long practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт или заменяет файл сдачи студента.
    /// </summary>
    Task<TaskFileSaveResult> SaveStudentTaskFileAsync(
        long taskId,
        long studentUserId,
        string storageKey,
        string originalFileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет комментарий преподавателя к файлу сдачи.
    /// </summary>
    Task<CaseFileComment> AddTeacherCommentAsync(
        long taskFileId,
        string comment,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Принимает файл сдачи и выставляет оценку.
    /// </summary>
    Task AcceptTaskFileAsync(long taskFileId, int grade, CancellationToken cancellationToken = default);
}
