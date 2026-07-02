using Education.Application.Theories;
using Microsoft.Extensions.Options;

namespace Education.Infrastructure.Courses;

internal sealed class PublicTheoryDocumentStorage(IOptions<PublicTheoryDocumentStorageOptions> options)
    : ITheoryDocumentStorage
{
    public async Task<string> SaveAsync(TheoryDocumentFile file, CancellationToken cancellationToken = default)
    {
        var safeFileName = Path.GetFileName(file.FileName);
        var dotIndex = safeFileName.LastIndexOf('.');
        var fileBase = dotIndex <= 0 ? safeFileName : safeFileName[..dotIndex];
        var fileExt = dotIndex <= 0 ? String.Empty : safeFileName[dotIndex..];
        var storedFileName = fileBase + "_" + Guid.NewGuid() + fileExt;
        var relativePath = Path.Combine(options.Value.DirectoryName, storedFileName);
        var absolutePath = Path.Combine(options.Value.RootPath, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        await using var stream = File.Create(absolutePath);
        await file.Content.CopyToAsync(stream, cancellationToken);

        return relativePath;
    }

    public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
    {
        var absolutePath = Path.Combine(options.Value.RootPath, path);
        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        return Task.CompletedTask;
    }
}
