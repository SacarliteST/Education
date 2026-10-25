using Education.Application.Users;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Users;

/// <summary>EF Core-реализация хранилища профилей пользователей.</summary>
internal sealed class EfUserProfileRepository : IUserProfileRepository
{
    private readonly EducationDbContext context;

    /// <summary>Создаёт экземпляр класса <see cref="EfUserProfileRepository"/>.</summary>
    /// <param name="context">Контекст базы данных Education.</param>
    public EfUserProfileRepository(EducationDbContext context)
    {
        this.context = context;
    }

    /// <inheritdoc />
    public Task<bool> IsActiveAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return context.Users
            .AsNoTracking()
            .AnyAsync(user => user.Id == userId && user.IsActive, cancellationToken);
    }

    /// <inheritdoc />
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
                user.IsActive,
                user.RoleId,
                user.GroupName))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
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
