using Education.Application.Identity;
using Education.Application.Users;

namespace Education.Application.Files;

public sealed class FilesService(
    ICurrentUser currentUser,
    IEducationUserResolver userResolver,
    IFileAccessRepository fileAccessRepository,
    IFileStorage fileStorage)
    : IFilesService
{
    private const string TeacherRole = "Teacher";
    private const string StudentRole = "Student";

    public async Task<StoredFileContent?> DownloadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        if (String.IsNullOrWhiteSpace(storageKey) || storageKey != Path.GetFileName(storageKey))
        {
            throw new FileStorageValidationException("Некорректный ключ файла.");
        }

        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        var fileInfo = await ResolveAccessibleFileAsync(storageKey, legacyUserId, cancellationToken);
        if (fileInfo is null)
        {
            throw new FileAccessDeniedException(storageKey);
        }

        var stream = await fileStorage.OpenReadAsync(fileInfo.StorageKey, cancellationToken);
        return stream is null ? null : new StoredFileContent(stream, fileInfo.OriginalFileName);
    }

    private async Task<StoredFileInfo?> ResolveAccessibleFileAsync(
        string storageKey,
        Guid legacyUserId,
        CancellationToken cancellationToken)
    {
        if (currentUser.Roles.Contains(StudentRole))
        {
            var taskFile = await fileAccessRepository.GetStudentTaskFileAsync(storageKey, legacyUserId, cancellationToken);
            if (taskFile is not null)
            {
                return taskFile;
            }

            var theoryDocument = await fileAccessRepository.GetStudentTheoryDocumentAsync(storageKey, legacyUserId, cancellationToken);
            if (theoryDocument is not null)
            {
                return theoryDocument;
            }
        }

        if (currentUser.Roles.Contains(TeacherRole))
        {
            var taskFile = await fileAccessRepository.GetTeacherTaskFileAsync(storageKey, legacyUserId, cancellationToken);
            if (taskFile is not null)
            {
                return taskFile;
            }

            return await fileAccessRepository.GetTeacherTheoryDocumentAsync(storageKey, legacyUserId, cancellationToken);
        }

        return null;
    }
}

