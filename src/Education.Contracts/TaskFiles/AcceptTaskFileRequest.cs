namespace Education.Contracts.TaskFiles;

/// <summary>
/// Запрос на принятие файла сдачи и выставление оценки.
/// </summary>
/// <param name="Grade">Оценка за сдачу.</param>
public sealed record AcceptTaskFileRequest(int Grade);


