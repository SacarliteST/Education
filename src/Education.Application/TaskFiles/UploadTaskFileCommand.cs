using Education.Application.Files;

namespace Education.Application.TaskFiles;

/// <summary>
/// Команда загрузки файла сдачи задания студентом.
/// </summary>
/// <param name="TaskId">Идентификатор задания.</param>
/// <param name="File">Загружаемый файл.</param>
public sealed record UploadTaskFileCommand(long TaskId, UploadFile File);
