using Education.Application.Practicals.TaskAssignments;
using Education.Domain.Practicals;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Practicals;

/// <summary>EF Core-реализация хранилища назначений заданий модуля студентам на пару.</summary>
internal sealed class EfPracticalTaskAssignmentsRepository : IPracticalTaskAssignmentsRepository
{
    private readonly EducationDbContext db;

    /// <summary>Создаёт экземпляр класса <see cref="EfPracticalTaskAssignmentsRepository"/>.</summary>
    /// <param name="db">Контекст базы данных Education.</param>
    public EfPracticalTaskAssignmentsRepository(EducationDbContext db)
    {
        this.db = db;
    }

    /// <inheritdoc />
    public Task<PracticalTaskAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return db.PracticalTaskAssignments
            .AsNoTracking()
            .FirstOrDefaultAsync(assignment => assignment.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PracticalTaskAssignment>> ListAsync(
        Guid practicalId, Guid classWindowId, CancellationToken cancellationToken = default)
    {
        return await db.PracticalTaskAssignments
            .AsNoTracking()
            .Where(assignment => assignment.PracticalMaterialId == practicalId
                && assignment.ClassWindowId == classWindowId)
            .OrderBy(assignment => assignment.StudentId)
            .ThenBy(assignment => assignment.SortOrder)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PracticalTaskAssignment>> ListForStudentAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default)
    {
        return await db.PracticalTaskAssignments
            .AsNoTracking()
            .Where(assignment => assignment.PracticalMaterialId == practicalId
                && assignment.ClassWindowId == classWindowId
                && assignment.StudentId == studentId)
            .OrderBy(assignment => assignment.SortOrder)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> ExistsForStudentAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default)
    {
        return db.PracticalTaskAssignments
            .AnyAsync(assignment => assignment.PracticalMaterialId == practicalId
                && assignment.ClassWindowId == classWindowId
                && assignment.StudentId == studentId,
                cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> HasSessionsForStudentAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default)
    {
        return (
            from session in db.PracticalModuleSessions
            join assignment in db.PracticalTaskAssignments
                on session.AssignmentId equals (Guid?)assignment.Id
            where assignment.PracticalMaterialId == practicalId
                && assignment.ClassWindowId == classWindowId
                && assignment.StudentId == studentId
            select session.Id)
            .AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> HasAnySessionsForPracticalAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        return (
            from session in db.PracticalModuleSessions
            join assignment in db.PracticalTaskAssignments
                on session.AssignmentId equals (Guid?)assignment.Id
            where assignment.PracticalMaterialId == practicalId
            select session.Id)
            .AnyAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Guid?> FindLatestWindowIdForStudentAsync(
        Guid practicalId, Guid studentId, CancellationToken cancellationToken = default)
    {
        return await (
            from assignment in db.PracticalTaskAssignments
            join window in db.ClassWindows on assignment.ClassWindowId equals window.Id
            where assignment.PracticalMaterialId == practicalId && assignment.StudentId == studentId
            orderby window.StartsAt descending
            select (Guid?)assignment.ClassWindowId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddRangeAsync(IEnumerable<PracticalTaskAssignment> assignments, CancellationToken cancellationToken = default)
    {
        await db.PracticalTaskAssignments.AddRangeAsync(assignments, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteForStudentAsync(
        Guid practicalId, Guid classWindowId, Guid studentId, CancellationToken cancellationToken = default)
    {
        await db.PracticalTaskAssignments
            .Where(assignment => assignment.PracticalMaterialId == practicalId
                && assignment.ClassWindowId == classWindowId
                && assignment.StudentId == studentId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}
