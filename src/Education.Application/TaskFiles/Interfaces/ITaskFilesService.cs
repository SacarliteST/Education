using Education.Domain.Practicals;

namespace Education.Application.TaskFiles;

/// <summary>
/// Выполняет сценарии работы с файлами сдачи заданий.
/// </summary>
public interface ITaskFilesService
{
    /// <summary>
    /// Возвращает файл сдачи текущего студента по заданию.
    /// </summary>
    Task<CaseFile?> GetStudentTaskFileAsync(Guid taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает файлы сдачи задания для текущего преподавателя.
    /// </summary>
    Task<IReadOnlyList<CaseFile>> GetTeacherTaskFilesAsync(Guid taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает файлы сдачи практического материала для текущего преподавателя.
    /// </summary>
    Task<IReadOnlyList<CaseFile>> GetTeacherPracticalTaskFilesAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Загружает или заменяет файл сдачи текущего студента.
    /// </summary>
    Task<CaseFile> UploadStudentTaskFileAsync(UploadTaskFileCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет комментарий текущего преподавателя к файлу сдачи.
    /// </summary>
    Task<CaseFileComment> AddTeacherCommentAsync(AddTaskFileCommentCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Принимает файл сдачи текущим преподавателем.
    /// </summary>
    Task AcceptTaskFileAsync(AcceptTaskFileCommand command, CancellationToken cancellationToken = default);
}

