namespace Education.Application.Files;

/// <summary>
/// Возникает, когда файл не проходит правила безопасного хранения.
/// </summary>
public sealed class FileStorageValidationException(string message) : InvalidOperationException(message);
