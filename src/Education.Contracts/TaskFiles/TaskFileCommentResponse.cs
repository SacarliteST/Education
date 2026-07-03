namespace Education.Contracts.TaskFiles;

/// <summary>
/// Комментарий к файлу сдачи задания.
/// </summary>
/// <param name="Id">Идентификатор комментария.</param>
/// <param name="Text">Текст комментария.</param>
/// <param name="Created">Дата и время создания комментария.</param>
/// <param name="IsGenerated">Признак системного комментария.</param>
public sealed record TaskFileCommentResponse(Guid Id, string Text, DateTime Created, bool IsGenerated);


