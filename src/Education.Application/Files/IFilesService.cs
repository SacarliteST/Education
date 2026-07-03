namespace Education.Application.Files;

/// <summary>
/// Выполняет сценарии безопасного скачивания файлов.
/// </summary>
public interface IFilesService
{
    /// <summary>
    /// Открывает файл для скачивания после проверки прав текущего пользователя.
    /// </summary>
    Task<StoredFileContent?> DownloadAsync(string storageKey, CancellationToken cancellationToken = default);
}

