namespace Education.Infrastructure.Courses;

public sealed class PublicTheoryDocumentStorageOptions
{
    public string RootPath { get; set; } = Directory.GetCurrentDirectory();

    public string DirectoryName { get; set; } = "Files";
}
