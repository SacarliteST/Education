namespace Education.Application.Files;

/// <summary>
/// Управляет физическим хранением файлов без привязки к конкретному сценарию приложения.
/// </summary>
public interface IFileStorage
{
    /// <summary>
    /// Сохраняет файл и возвращает безопасный серверный ключ.
    /// </summary>
    Task<StoredFileInfo> SaveAsync(UploadFile file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Открывает файл для чтения по безопасному серверному ключу.
    /// </summary>
    Task<Stream?> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет файл по безопасному серверному ключу.
    /// </summary>
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
}

