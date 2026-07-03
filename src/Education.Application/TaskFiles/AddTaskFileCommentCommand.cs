namespace Education.Application.TaskFiles;

/// <summary>
/// Команда добавления комментария преподавателя к файлу сдачи.
/// </summary>
/// <param name="TaskFileId">Идентификатор файла сдачи.</param>
/// <param name="Comment">Текст комментария.</param>
public sealed record AddTaskFileCommentCommand(Guid TaskFileId, string Comment);

