using Education.Application.PracticalModules;
using Education.Domain.PracticalModules;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.PracticalModules;

internal sealed class EfPracticalModuleSessionsRepository(EducationDbContext context)
    : IPracticalModuleSessionsRepository
{
    public Task<PracticalModuleSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return context.PracticalModuleSessions.FirstOrDefaultAsync(session => session.Id == id, cancellationToken);
    }

    public Task<PracticalModuleSession?> GetLatestForUserTaskAsync(
        Guid userId,
        Guid practicalTaskId,
        CancellationToken cancellationToken = default)
    {
        return context.PracticalModuleSessions
            .Where(session => session.UserId == userId && session.PracticalTaskId == practicalTaskId)
            .OrderByDescending(session => session.StartedAt)
            .ThenByDescending(session => session.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int> CountForUserTaskAsync(
        Guid userId,
        Guid practicalTaskId,
        CancellationToken cancellationToken = default)
    {
        return context.PracticalModuleSessions
            .CountAsync(
                session => session.UserId == userId && session.PracticalTaskId == practicalTaskId,
                cancellationToken);
    }

    public async Task<int?> BestGradeForUserTaskAsync(
        Guid userId,
        Guid practicalTaskId,
        CancellationToken cancellationToken = default)
    {
        return await context.PracticalModuleSessions
            .Where(session => session.UserId == userId
                && session.PracticalTaskId == practicalTaskId
                && session.Status == ModuleSessionState.Completed
                && session.Grade != null)
            .MaxAsync(session => (int?)session.Grade, cancellationToken);
    }

    public async Task<IReadOnlyList<ModuleSessionSummary>> ListForPracticalAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        var caseIds = context.Cases
            .Where(item => item.PracticalMaterialId == practicalId)
            .Select(item => item.Id);

        var query =
            from session in context.PracticalModuleSessions.AsNoTracking()
            where caseIds.Contains(session.PracticalTaskId)
            join candidate in context.Users.AsNoTracking() on session.UserId equals candidate.Id into joined
            from user in joined.DefaultIfEmpty()
            orderby session.StartedAt descending, session.Id descending
            select new
            {
                session.Id,
                session.UserId,
                UserLastName = user != null ? user.LastName : null,
                UserFirstName = user != null ? user.FirstName : null,
                UserMiddleName = user != null ? user.MiddleName : null,
                session.TryNumber,
                session.Status,
                session.EndReason,
                session.Grade,
                session.StartedAt,
                session.EndedAt,
            };

        var rows = await query.ToListAsync(cancellationToken);

        return rows
            .Select(row => new ModuleSessionSummary(
                row.Id,
                row.UserId,
                BuildName(row.UserLastName, row.UserFirstName, row.UserMiddleName),
                row.TryNumber,
                row.Status,
                row.EndReason,
                row.Grade,
                row.StartedAt,
                row.EndedAt))
            .ToList();
    }

    private static string BuildName(string? lastName, string? firstName, string? middleName)
    {
        var name = String.Join(' ', new[] { lastName, firstName, middleName }
            .Where(part => !String.IsNullOrWhiteSpace(part)));
        return String.IsNullOrWhiteSpace(name) ? "—" : name;
    }

    public async Task AddAsync(PracticalModuleSession session, CancellationToken cancellationToken = default)
    {
        await context.PracticalModuleSessions.AddAsync(session, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
