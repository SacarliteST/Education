namespace Education.Infrastructure.Files;

/// <summary>
/// Настройки локального файлового хранилища учебных материалов и решений.
/// </summary>
public sealed class FileStorageOptions
{
    /// <summary>
    /// Корневой каталог, в котором сохраняются файлы.
    /// </summary>
    public string RootPath { get; set; } = String.Empty;

    /// <summary>
    /// Максимальный допустимый размер загружаемого файла в байтах.
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;

    /// <summary>
    /// Разрешённые расширения загружаемых файлов.
    /// </summary>
    public string[] AllowedExtensions { get; set; } =
    [
        ".doc",
        ".docx",
        ".pdf",
        ".ppt",
        ".pptx",
        ".xls",
        ".xlsx",
        ".txt",
        ".png",
        ".jpg",
        ".jpeg",
        ".zip"
    ];
}


