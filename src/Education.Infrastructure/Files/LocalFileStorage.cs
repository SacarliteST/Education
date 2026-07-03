using Education.Application.Files;
using Microsoft.Extensions.Options;

namespace Education.Infrastructure.Files;

internal sealed class LocalFileStorage(IOptions<FileStorageOptions> options) : IFileStorage
{
    public async Task<StoredFileInfo> SaveAsync(UploadFile file, CancellationToken cancellationToken = default)
    {
        var safeOriginalName = Path.GetFileName(file.OriginalFileName);
        if (String.IsNullOrWhiteSpace(safeOriginalName))
        {
            throw new FileStorageValidationException("Имя файла не указано.");
        }

        if (file.Length <= 0)
        {
            throw new FileStorageValidationException("Файл пуст.");
        }

        if (file.Length > options.Value.MaxFileSizeBytes)
        {
            throw new FileStorageValidationException("Размер файла превышает допустимый лимит.");
        }

        var extension = Path.GetExtension(safeOriginalName).ToLowerInvariant();
        var allowedExtensions = options.Value.AllowedExtensions
            .Select(item => item.StartsWith('.') ? item.ToLowerInvariant() : "." + item.ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (String.IsNullOrWhiteSpace(extension) || !allowedExtensions.Contains(extension))
        {
            throw new FileStorageValidationException("Расширение файла не разрешено.");
        }

        var storageKey = Guid.NewGuid().ToString("N") + extension;
        var destinationPath = GetSafePath(storageKey);

        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

        await using var destination = File.Create(destinationPath);
        await file.Content.CopyToAsync(destination, cancellationToken);

        return new StoredFileInfo(storageKey, safeOriginalName);
    }

    public Task<Stream?> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var path = GetSafePath(storageKey);
        Stream? stream = File.Exists(path) ? File.OpenRead(path) : null;
        return Task.FromResult(stream);
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var path = GetSafePath(storageKey);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }

    private string GetSafePath(string storageKey)
    {
        if (String.IsNullOrWhiteSpace(storageKey) || storageKey != Path.GetFileName(storageKey))
        {
            throw new FileStorageValidationException("Некорректный ключ файла.");
        }

        var root = Path.GetFullPath(options.Value.RootPath);
        var path = Path.GetFullPath(Path.Combine(root, storageKey));

        if (!path.StartsWith(root, StringComparison.OrdinalIgnoreCase))
        {
            throw new FileStorageValidationException("Некорректный ключ файла.");
        }

        return path;
    }
}


