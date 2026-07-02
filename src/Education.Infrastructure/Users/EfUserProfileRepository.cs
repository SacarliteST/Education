using Education.Application.Users;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Users;

internal sealed class EfUserProfileRepository(EducationDbContext context) : IUserProfileRepository
{
    public Task<long?> FindLegacyUserIdByIdentityUserIdAsync(
        Guid identityUserId,
        CancellationToken cancellationToken = default)
    {
        return context.IdentityUserLinks
            .AsNoTracking()
            .Where(link => link.IdentityUserId == identityUserId && link.IsActive)
            .Select(link => (long?)link.LegacyUserId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserProfile?> FindProfileByLegacyUserIdAsync(
        long legacyUserId,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()
            .GroupJoin(
                context.IdentityUserLinks.Where(link => link.IsActive),
                user => user.Id,
                link => link.LegacyUserId,
                (user, links) => new { user, link = links.FirstOrDefault() })
            .Where(item => item.user.Id == legacyUserId)
            .Select(item => new UserProfile(
                item.user.Id,
                item.link == null ? null : item.link.IdentityUserId,
                item.user.Login,
                item.user.FirstName,
                item.user.LastName,
                item.user.MiddleName,
                item.link == null || item.link.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserRelationsSnapshot> GetRelationsSnapshotAsync(
        long legacyUserId,
        CancellationToken cancellationToken = default)
    {
        var ownedCourseIds = await context.Courses
            .AsNoTracking()
            .Where(course => course.UserId == legacyUserId)
            .Select(course => course.Id)
            .ToHashSetAsync(cancellationToken);

        var assignedCourseIds = await context.CourseBindUsers
            .AsNoTracking()
            .Where(bind => bind.UserId == legacyUserId)
            .Select(bind => bind.CourseId)
            .ToHashSetAsync(cancellationToken);

        var assignedPracticalIds = await context.PracticalBindUsers
            .AsNoTracking()
            .Where(bind => bind.UserId == legacyUserId)
            .Select(bind => bind.PracticalMaterialId)
            .ToHashSetAsync(cancellationToken);

        var caseFileIds = await context.CaseFiles
            .AsNoTracking()
            .Where(file => file.UserId == legacyUserId)
            .Select(file => file.Id)
            .ToHashSetAsync(cancellationToken);

        var testResultIds = await context.TestResults
            .AsNoTracking()
            .Where(result => result.UserId == legacyUserId)
            .Select(result => result.Id)
            .ToHashSetAsync(cancellationToken);

        return new UserRelationsSnapshot(
            legacyUserId,
            ownedCourseIds,
            assignedCourseIds,
            assignedPracticalIds,
            caseFileIds,
            testResultIds);
    }
}
