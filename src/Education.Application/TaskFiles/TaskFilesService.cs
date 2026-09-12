using Education.Application.Courses;
using Education.Application.Files;
using Education.Application.TestResults;
using Education.Application.Users;
using Education.Domain.Practicals;
using Microsoft.Extensions.Logging;

namespace Education.Application.TaskFiles;

public sealed class TaskFilesService(
    IEducationUserResolver userResolver,
    ITaskFilesRepository taskFilesRepository,
    IFileStorage fileStorage,
    ILogger<TaskFilesService> logger)
    : ITaskFilesService
{
    public async Task<CaseFile?> GetStudentTaskFileAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await taskFilesRepository.IsTaskAssignedToStudentAsync(taskId, legacyUserId, cancellationToken))
        {
            throw new StudentPracticalAccessDeniedException(taskId);
        }

        return await taskFilesRepository.GetStudentTaskFileAsync(taskId, legacyUserId, cancellationToken);
    }

    public async Task<IReadOnlyList<CaseFile>> GetTeacherTaskFilesAsync(
        Guid taskId,
        CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await taskFilesRepository.IsTaskOwnedByTeacherAsync(taskId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(taskId);
        }

        return await taskFilesRepository.GetTeacherTaskFilesAsync(taskId, cancellationToken);
    }

    public async Task<IReadOnlyList<CaseFile>> GetTeacherPracticalTaskFilesAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await taskFilesRepository.IsPracticalOwnedByTeacherAsync(practicalId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(practicalId);
        }

        return await taskFilesRepository.GetTeacherPracticalTaskFilesAsync(practicalId, cancellationToken);
    }

    public async Task<CaseFile> UploadStudentTaskFileAsync(
        UploadTaskFileCommand command,
        CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await taskFilesRepository.IsTaskAssignedToStudentAsync(command.TaskId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в загрузке файла сдачи по заданию {TaskId}: студент {UserId} не назначен.",
                command.TaskId, legacyUserId);
            throw new StudentPracticalAccessDeniedException(command.TaskId);
        }

        var storedFile = await fileStorage.SaveAsync(command.File, cancellationToken);
        var result = await taskFilesRepository.SaveStudentTaskFileAsync(
            command.TaskId,
            legacyUserId,
            storedFile.StorageKey,
            storedFile.OriginalFileName,
            cancellationToken);

        if (result.ReplacedStorageKey is not null)
        {
            await fileStorage.DeleteAsync(result.ReplacedStorageKey, cancellationToken);
        }

        logger.LogInformation(
            "Студент {UserId} загрузил файл сдачи «{FileName}» по заданию {TaskId} " +
            "({ReplacedPrevious}).",
            legacyUserId, storedFile.OriginalFileName, command.TaskId, result.ReplacedStorageKey is not null ? "замена предыдущего" : "первая сдача");
        return result.TaskFile;
    }

    public async Task<CaseFileComment> AddTeacherCommentAsync(
        AddTaskFileCommentCommand command,
        CancellationToken cancellationToken = default)
    {
        await EnsureTaskFileOwnerAsync(command.TaskFileId, cancellationToken);
        return await taskFilesRepository.AddTeacherCommentAsync(command.TaskFileId, command.Comment, cancellationToken);
    }

    public async Task AcceptTaskFileAsync(AcceptTaskFileCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureTaskFileOwnerAsync(command.TaskFileId, cancellationToken);
        await taskFilesRepository.AcceptTaskFileAsync(command.TaskFileId, command.Grade, cancellationToken);
        logger.LogInformation(
            "Файл сдачи {TaskFileId} принят с оценкой {Grade}.", command.TaskFileId, command.Grade);
    }

    private async Task EnsureTaskFileOwnerAsync(Guid taskFileId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await taskFilesRepository.IsTaskFileOwnedByTeacherAsync(taskFileId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в доступе к файлу сдачи {TaskFileId}: пользователь {LegacyUserId} не владелец.",
                taskFileId, legacyUserId);
            throw new CourseAccessDeniedException(taskFileId);
        }
    }
}
