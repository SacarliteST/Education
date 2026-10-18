using Education.Application.PracticalModules;
using Education.Infrastructure.Users;
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
                session.CompletionData,
            };

        var rows = await query.ToListAsync(cancellationToken);

        return rows
            .Select(row => new ModuleSessionSummary(
                row.Id,
                row.UserId,
                PersonNameFormatter.Format(row.UserLastName, row.UserFirstName, row.UserMiddleName),
                row.TryNumber,
                row.Status,
                row.EndReason,
                row.Grade,
                row.StartedAt,
                row.EndedAt,
                ReadAttemptsCount(row.CompletionData)))
            .ToList();
    }

    /// <summary>
    /// Число попыток внутри сессии из <c>CompletionData.totalAttempts</c> (присылает модуль при завершении).
    /// Нет данных (сессия не завершена, другой формат) — <c>null</c>.
    /// </summary>
    private static int? ReadAttemptsCount(string? completionData)
    {
        if (String.IsNullOrWhiteSpace(completionData))
        {
            return null;
        }

        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(completionData);
            return document.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object
                && document.RootElement.TryGetProperty("totalAttempts", out var value)
                && value.ValueKind == System.Text.Json.JsonValueKind.Number
                && value.TryGetInt32(out var count)
                && count >= 0
                    ? count
                    : null;
        }
        catch (System.Text.Json.JsonException)
        {
            return null;
        }
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
