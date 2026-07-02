namespace Education.Contracts.TaskFiles;

/// <summary>
/// Данные файла сдачи задания.
/// </summary>
/// <param name="Id">Идентификатор файла сдачи.</param>
/// <param name="StorageKey">Серверный ключ файла для скачивания через API.</param>
/// <param name="Name">Исходное имя файла.</param>
/// <param name="UserId">Идентификатор студента, загрузившего файл.</param>
/// <param name="FullName">Полное имя студента.</param>
/// <param name="IsAccepted">Признак принятой сдачи.</param>
/// <param name="IsUpdated">Признак последнего системного обновления файла.</param>
/// <param name="Grade">Оценка за сдачу.</param>
/// <param name="Comments">Комментарии к файлу сдачи.</param>
public sealed record TaskFileResponse(
    long Id,
    string StorageKey,
    string Name,
    long UserId,
    string FullName,
    bool IsAccepted,
    bool IsUpdated,
    int Grade,
    IReadOnlyList<TaskFileCommentResponse> Comments);
