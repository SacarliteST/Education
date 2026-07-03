namespace Education.Contracts.TaskFiles;

/// <summary>
/// Запрос на добавление комментария преподавателя к файлу сдачи.
/// </summary>
/// <param name="Comment">Текст комментария.</param>
public sealed record AddTaskFileCommentRequest(string Comment);


