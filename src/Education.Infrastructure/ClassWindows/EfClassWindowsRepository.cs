using Education.Application.ClassWindows;
using Education.Domain.Courses;
using Education.Domain.PracticalModules;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.ClassWindows;

internal sealed class EfClassWindowsRepository(EducationDbContext db) : IClassWindowsRepository
{
    public Task<PracticeGate?> GetGateForPracticalAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        return db.PracticalMaterials
            .AsNoTracking()
            .Where(practical => practical.Id == practicalId)
            .Select(practical => new PracticeGate(
                practical.Module.CourseId, practical.Module.Course.PracticeOnlyDuringClass))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<PracticeGate?> GetGateForTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return db.Cases
            .AsNoTracking()
            .Where(task => task.Id == taskId)
            .Select(task => new PracticeGate(
                task.PracticalMaterial.Module.CourseId, task.PracticalMaterial.Module.Course.PracticeOnlyDuringClass))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<Course?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return db.Courses.FirstOrDefaultAsync(course => course.Id == courseId, cancellationToken);
    }

    public Task<ClassWindow?> FindOpenAsync(
        Guid courseId, string groupName, DateTimeOffset at, TimeSpan grace, CancellationToken cancellationToken = default)
    {
        var groupKey = groupName.Trim().ToLower();
        var endCheck = at - grace;
        return db.ClassWindows
            .AsNoTracking()
            .Where(window => window.CourseId == courseId
                && window.GroupName.ToLower() == groupKey
                && window.StartsAt <= at
                && window.EndsAt > endCheck
                && (window.ClosedAt == null || window.ClosedAt > endCheck))
            .OrderByDescending(window => window.StartsAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<ClassWindow?> GetUnclosedAsync(Guid courseId, string groupName, CancellationToken cancellationToken = default)
    {
        var groupKey = groupName.Trim();
        return db.ClassWindows.FirstOrDefaultAsync(
            window => window.CourseId == courseId && window.GroupName == groupKey && window.ClosedAt == null,
            cancellationToken);
    }

    public Task<ClassWindow?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return db.ClassWindows.FirstOrDefaultAsync(window => window.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ClassWindow>> ListForCourseAsync(
        Guid courseId, int take, CancellationToken cancellationToken = default)
    {
        return await db.ClassWindows
            .AsNoTracking()
            .Where(window => window.CourseId == courseId)
            .OrderByDescending(window => window.StartsAt)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> ListGroupsAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await db.CourseBindUsers
            .AsNoTracking()
            .Where(bind => bind.CourseId == courseId && bind.User.GroupName != null && bind.User.GroupName != "")
            .Select(bind => bind.User.GroupName!)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ActiveSessionTarget>> ListActiveSessionsAsync(
        Guid courseId, string groupName, CancellationToken cancellationToken = default)
    {
        var groupKey = groupName.Trim().ToLower();
        var rows = await (
            from session in db.PracticalModuleSessions
            join task in db.Cases on session.PracticalTaskId equals task.Id
            join user in db.Users on session.UserId equals user.Id
            where session.Status == ModuleSessionState.Active
                && task.PracticalModuleId != null
                && task.PracticalMaterial.Module.CourseId == courseId
                && user.GroupName != null
                && user.GroupName.ToLower() == groupKey
            select new
            {
                Session = session,
                PracticalModuleId = task.PracticalModuleId!.Value,
                task.ExternalTaskRef,
                task.PracticalMaterial.AttemptsPerSession,
                task.PracticalMaterial.TimeLimitMinutes,
            }).ToListAsync(cancellationToken);
        return rows
            .Select(row => new ActiveSessionTarget(
                row.Session, row.PracticalModuleId, row.ExternalTaskRef!, row.AttemptsPerSession, row.TimeLimitMinutes))
            .ToList();
    }

    public async Task AddAsync(ClassWindow window, CancellationToken cancellationToken = default)
    {
        await db.ClassWindows.AddAsync(window, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}
