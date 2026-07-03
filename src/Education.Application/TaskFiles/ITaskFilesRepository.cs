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
    Task<bool> IsTaskAssignedToStudentAsync(Guid taskId, Guid studentUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли задание курсу указанного преподавателя.
    /// </summary>
    Task<bool> IsTaskOwnedByTeacherAsync(Guid taskId, Guid teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли практический материал курсу указанного преподавателя.
    /// </summary>
    Task<bool> IsPracticalOwnedByTeacherAsync(Guid practicalId, Guid teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли файл сдачи курсу указанного преподавателя.
    /// </summary>
    Task<bool> IsTaskFileOwnedByTeacherAsync(Guid taskFileId, Guid teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает файл сдачи текущего студента по заданию.
    /// </summary>
    Task<CaseFile?> GetStudentTaskFileAsync(Guid taskId, Guid studentUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает файлы сдачи указанного задания для преподавателя.
    /// </summary>
    Task<IReadOnlyList<CaseFile>> GetTeacherTaskFilesAsync(Guid taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает файлы сдачи всех заданий практического материала для преподавателя.
    /// </summary>
    Task<IReadOnlyList<CaseFile>> GetTeacherPracticalTaskFilesAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт или заменяет файл сдачи студента.
    /// </summary>
    Task<TaskFileSaveResult> SaveStudentTaskFileAsync(
        Guid taskId,
        Guid studentUserId,
        string storageKey,
        string originalFileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет комментарий преподавателя к файлу сдачи.
    /// </summary>
    Task<CaseFileComment> AddTeacherCommentAsync(
        Guid taskFileId,
        string comment,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Принимает файл сдачи и выставляет оценку.
    /// </summary>
    Task AcceptTaskFileAsync(Guid taskFileId, int grade, CancellationToken cancellationToken = default);
}

