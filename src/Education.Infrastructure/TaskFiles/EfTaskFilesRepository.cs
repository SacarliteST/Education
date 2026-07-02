using Education.Application.TaskFiles;
using Education.Domain.Practicals;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.TaskFiles;

internal sealed class EfTaskFilesRepository(EducationDbContext context) : ITaskFilesRepository
{
    public Task<bool> IsTaskAssignedToStudentAsync(
        long taskId,
        long studentUserId,
        CancellationToken cancellationToken = default)
    {
        return context.Cases.AnyAsync(
            task => task.Id == taskId
                && task.PracticalMaterial.PracticalBindUsers.Any(bind => bind.UserId == studentUserId),
            cancellationToken);
    }

    public Task<bool> IsTaskOwnedByTeacherAsync(
        long taskId,
        long teacherUserId,
        CancellationToken cancellationToken = default)
    {
        return context.Cases.AnyAsync(
            task => task.Id == taskId && task.PracticalMaterial.Module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    public Task<bool> IsPracticalOwnedByTeacherAsync(
        long practicalId,
        long teacherUserId,
        CancellationToken cancellationToken = default)
    {
        return context.PracticalMaterials.AnyAsync(
            practical => practical.Id == practicalId && practical.Module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    public Task<bool> IsTaskFileOwnedByTeacherAsync(
        long taskFileId,
        long teacherUserId,
        CancellationToken cancellationToken = default)
    {
        return context.CaseFiles.AnyAsync(
            file => file.Id == taskFileId && file.Case.PracticalMaterial.Module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    public Task<CaseFile?> GetStudentTaskFileAsync(
        long taskId,
        long studentUserId,
        CancellationToken cancellationToken = default)
    {
        return IncludeDetails(context.CaseFiles.AsNoTracking())
            .FirstOrDefaultAsync(file => file.CaseId == taskId && file.UserId == studentUserId, cancellationToken);
    }

    public async Task<IReadOnlyList<CaseFile>> GetTeacherTaskFilesAsync(
        long taskId,
        CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(context.CaseFiles.AsNoTracking())
            .Where(file => file.CaseId == taskId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CaseFile>> GetTeacherPracticalTaskFilesAsync(
        long practicalId,
        CancellationToken cancellationToken = default)
    {
        return await IncludeDetails(context.CaseFiles.AsNoTracking())
            .Where(file => file.Case.PracticalMaterialId == practicalId)
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskFileSaveResult> SaveStudentTaskFileAsync(
        long taskId,
        long studentUserId,
        string storageKey,
        string originalFileName,
        CancellationToken cancellationToken = default)
    {
        var taskFile = await context.CaseFiles
            .Include(file => file.Comments)
            .FirstOrDefaultAsync(file => file.CaseId == taskId && file.UserId == studentUserId, cancellationToken);

        string? replacedStorageKey = null;
        var generatedComment = "Файл добавлен";

        if (taskFile is null)
        {
            taskFile = new CaseFile(taskId, studentUserId, storageKey, originalFileName);
            await context.CaseFiles.AddAsync(taskFile, cancellationToken);
        }
        else
        {
            replacedStorageKey = taskFile.Path;
            taskFile.ReplaceFile(storageKey, originalFileName);
            generatedComment = "Файл обновлен";
        }

        await context.SaveChangesAsync(cancellationToken);

        await context.CaseFileComments.AddAsync(
            new CaseFileComment(taskFile.Id, generatedComment, true),
            cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var savedTaskFile = await IncludeDetails(context.CaseFiles.AsNoTracking())
            .FirstAsync(file => file.Id == taskFile.Id, cancellationToken);

        return new TaskFileSaveResult(savedTaskFile, replacedStorageKey);
    }

    public async Task<CaseFileComment> AddTeacherCommentAsync(
        long taskFileId,
        string comment,
        CancellationToken cancellationToken = default)
    {
        var taskFileComment = new CaseFileComment(taskFileId, comment, false);
        await context.CaseFileComments.AddAsync(taskFileComment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return taskFileComment;
    }

    public async Task AcceptTaskFileAsync(long taskFileId, int grade, CancellationToken cancellationToken = default)
    {
        var taskFile = await context.CaseFiles.FirstOrDefaultAsync(file => file.Id == taskFileId, cancellationToken);
        taskFile?.Accept(grade);
        await context.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<CaseFile> IncludeDetails(IQueryable<CaseFile> query)
    {
        return query
            .Include(file => file.User)
            .Include(file => file.Comments)
            .Include(file => file.Case);
    }
}
