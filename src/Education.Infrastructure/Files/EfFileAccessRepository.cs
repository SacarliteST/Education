using Education.Application.Files;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Files;

/// <summary>EF Core-реализация хранилища данных для проверки доступа к файлам.</summary>
internal sealed class EfFileAccessRepository : IFileAccessRepository
{
    private readonly EducationDbContext context;

    /// <summary>Создаёт экземпляр класса <see cref="EfFileAccessRepository"/>.</summary>
    /// <param name="context">Контекст базы данных Education.</param>
    public EfFileAccessRepository(EducationDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public Task<StoredFileInfo?> GetStudentTaskFileAsync(
        string storageKey,
        Guid studentUserId,
        CancellationToken cancellationToken = default)
    {
        return context.CaseFiles
            .AsNoTracking()
            .Where(file => file.Path == storageKey && file.UserId == studentUserId)
            .Select(file => new StoredFileInfo(file.Path, file.OriginalFileName))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<StoredFileInfo?> GetTeacherTaskFileAsync(
        string storageKey,
        Guid teacherUserId,
        CancellationToken cancellationToken = default)
    {
        return context.CaseFiles
            .AsNoTracking()
            .Where(file => file.Path == storageKey && file.Case.PracticalMaterial.Module.Course.UserId == teacherUserId)
            .Select(file => new StoredFileInfo(file.Path, file.OriginalFileName))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<StoredFileInfo?> GetStudentTheoryDocumentAsync(
        string storageKey,
        Guid studentUserId,
        CancellationToken cancellationToken = default)
    {
        return context.TheoreticalMaterialFiles
            .AsNoTracking()
            .Where(file => file.Path == storageKey
                && file.TheoreticalMaterial.Module.Course.CourseBindUsers.Any(bind => bind.UserId == studentUserId))
            .Select(file => new StoredFileInfo(file.Path, file.OriginalFileName))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<StoredFileInfo?> GetTeacherTheoryDocumentAsync(
        string storageKey,
        Guid teacherUserId,
        CancellationToken cancellationToken = default)
    {
        return context.TheoreticalMaterialFiles
            .AsNoTracking()
            .Where(file => file.Path == storageKey && file.TheoreticalMaterial.Module.Course.UserId == teacherUserId)
            .Select(file => new StoredFileInfo(file.Path, file.OriginalFileName))
            .FirstOrDefaultAsync(cancellationToken);
    }
}


