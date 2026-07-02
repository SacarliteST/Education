namespace Education.Application.Files;

/// <summary>
/// Возникает, когда текущий пользователь не имеет доступа к запрошенному файлу.
/// </summary>
public sealed class FileAccessDeniedException(string storageKey)
    : UnauthorizedAccessException($"Current user does not have access to file '{storageKey}'.")
{
    /// <summary>
    /// Серверный ключ файла, доступ к которому был запрещён.
    /// </summary>
    public string StorageKey { get; } = storageKey;
}
