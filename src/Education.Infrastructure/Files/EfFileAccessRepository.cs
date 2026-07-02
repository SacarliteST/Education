using Education.Application.Files;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Files;

internal sealed class EfFileAccessRepository(EducationDbContext context) : IFileAccessRepository
{
    public Task<StoredFileInfo?> GetStudentTaskFileAsync(
        string storageKey,
        long studentUserId,
        CancellationToken cancellationToken = default)
    {
        return context.CaseFiles
            .AsNoTracking()
            .Where(file => file.Path == storageKey && file.UserId == studentUserId)
            .Select(file => new StoredFileInfo(file.Path, file.OriginalFileName))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<StoredFileInfo?> GetTeacherTaskFileAsync(
        string storageKey,
        long teacherUserId,
        CancellationToken cancellationToken = default)
    {
        return context.CaseFiles
            .AsNoTracking()
            .Where(file => file.Path == storageKey && file.Case.PracticalMaterial.Module.Course.UserId == teacherUserId)
            .Select(file => new StoredFileInfo(file.Path, file.OriginalFileName))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<StoredFileInfo?> GetStudentTheoryDocumentAsync(
        string storageKey,
        long studentUserId,
        CancellationToken cancellationToken = default)
    {
        return context.TheoreticalMaterialFiles
            .AsNoTracking()
            .Where(file => file.Path == storageKey
                && file.TheoreticalMaterial.Module.Course.CourseBindUsers.Any(bind => bind.UserId == studentUserId))
            .Select(file => new StoredFileInfo(file.Path, file.OriginalFileName))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<StoredFileInfo?> GetTeacherTheoryDocumentAsync(
        string storageKey,
        long teacherUserId,
        CancellationToken cancellationToken = default)
    {
        return context.TheoreticalMaterialFiles
            .AsNoTracking()
            .Where(file => file.Path == storageKey && file.TheoreticalMaterial.Module.Course.UserId == teacherUserId)
            .Select(file => new StoredFileInfo(file.Path, file.OriginalFileName))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
