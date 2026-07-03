namespace Education.Application.TaskFiles;

/// <summary>
/// Команда принятия файла сдачи и выставления оценки.
/// </summary>
/// <param name="TaskFileId">Идентификатор файла сдачи.</param>
/// <param name="Grade">Оценка за сдачу.</param>
public sealed record AcceptTaskFileCommand(Guid TaskFileId, int Grade);

