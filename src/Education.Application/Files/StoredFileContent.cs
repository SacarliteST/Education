namespace Education.Application.Files;

/// <summary>
/// Содержит поток сохранённого файла и имя, с которым файл отдаётся клиенту.
/// </summary>
/// <param name="Content">Поток с содержимым файла.</param>
/// <param name="OriginalFileName">Исходное имя файла.</param>
public sealed record StoredFileContent(Stream Content, string OriginalFileName);

