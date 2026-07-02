namespace Education.Infrastructure.Files;

public sealed class FileStorageOptions
{
    public string RootPath { get; set; } = String.Empty;

    public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024;

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
