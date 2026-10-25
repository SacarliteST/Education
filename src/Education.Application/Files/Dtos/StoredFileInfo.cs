namespace Education.Application.Files;

/// <summary>
/// Описывает файл после сохранения в физическом хранилище.
/// </summary>
/// <param name="StorageKey">Серверный ключ файла в хранилище.</param>
/// <param name="OriginalFileName">Исходное имя файла, полученное от клиента.</param>
public sealed record StoredFileInfo(string StorageKey, string OriginalFileName);

