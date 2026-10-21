using Education.Application.Users;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Users;

internal sealed class EfUserProfileRepository(EducationDbContext context) : IUserProfileRepository
{
    public Task<bool> IsActiveAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return context.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == userId && user.IsActive, cancellationToken);
    }

    public async Task<UserProfile?> FindProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()
            .Where(user => user.Id == userId)
            .Select(user => new UserProfile(
                user.Id,
                user.Login,
                user.FirstName,
                user.LastName,
                user.MiddleName,
                user.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserRelationsSnapshot> GetRelationsSnapshotAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var ownedCourseIds = await context.Courses
            .AsNoTracking()
            .Where(course => course.UserId == userId)
            .Select(course => course.Id)
            .ToHashSetAsync(cancellationToken);

        var assignedCourseIds = await context.CourseBindUsers
            .AsNoTracking()
            .Where(bind => bind.UserId == userId)
            .Select(bind => bind.CourseId)
            .ToHashSetAsync(cancellationToken);

        var assignedPracticalIds = await context.PracticalBindUsers
            .AsNoTracking()
            .Where(bind => bind.UserId == userId)
            .Select(bind => bind.PracticalMaterialId)
            .ToHashSetAsync(cancellationToken);

        var caseFileIds = await context.CaseFiles
            .AsNoTracking()
            .Where(file => file.UserId == userId)
            .Select(file => file.Id)
            .ToHashSetAsync(cancellationToken);

        var testResultIds = await context.TestResults
            .AsNoTracking()
            .Where(result => result.UserId == userId)
            .Select(result => result.Id)
            .ToHashSetAsync(cancellationToken);

        return new UserRelationsSnapshot(
            userId,
            ownedCourseIds,
            assignedCourseIds,
            assignedPracticalIds,
            caseFileIds,
            testResultIds);
    }
}
