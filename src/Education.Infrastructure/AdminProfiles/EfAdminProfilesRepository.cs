using Education.Application.AdminProfiles;
using Education.Domain.Users;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.AdminProfiles;

internal sealed class EfAdminProfilesRepository(EducationDbContext context) : IAdminProfilesRepository
{
    public async Task<IReadOnlyList<AdminProfile>> GetProfilesAsync(CancellationToken cancellationToken = default)
    {
        return await ProfilesQuery()
            .OrderBy(profile => profile.LastName)
            .ThenBy(profile => profile.FirstName)
            .ToListAsync(cancellationToken);
    }

    public async Task<AdminProfile> CreateLinkedProfileAsync(
        CreateAdminProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var identityAlreadyLinked = await context.IdentityUserLinks.AnyAsync(
            link => link.IdentityUserId == command.IdentityUserId && link.IsActive,
            cancellationToken);
        var loginAlreadyExists = await context.Users.AnyAsync(
            user => user.Login == command.Login,
            cancellationToken);
        if (identityAlreadyLinked || loginAlreadyExists)
        {
            throw new AdminProfileAlreadyLinkedException();
        }

        var user = User.CreateLegacyProfile(
            command.Login,
            command.FirstName,
            command.LastName,
            command.MiddleName);
        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await context.IdentityUserLinks.AddAsync(
            new IdentityUserLink
            {
                LegacyUserId = user.Id,
                IdentityUserId = command.IdentityUserId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
            cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return await ProfilesQuery(user.Id).FirstAsync(cancellationToken);
    }

    public async Task<AdminProfile?> UpdateProfileAsync(
        Guid legacyUserId,
        UpdateAdminProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FirstOrDefaultAsync(item => item.Id == legacyUserId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        user.UpdateDisplayName(command.Login, command.FirstName, command.LastName, command.MiddleName);
        await context.SaveChangesAsync(cancellationToken);

        return await ProfilesQuery(legacyUserId).FirstAsync(cancellationToken);
    }

    public async Task<bool> DeactivateProfileLinkAsync(Guid legacyUserId, CancellationToken cancellationToken = default)
    {
        var link = await context.IdentityUserLinks.FirstOrDefaultAsync(
            item => item.LegacyUserId == legacyUserId && item.IsActive,
            cancellationToken);
        if (link is null)
        {
            return false;
        }

        link.IsActive = false;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<AssignableStudent>> GetAssignableStudentsForCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()
            .Where(user => context.IdentityUserLinks.Any(link => link.LegacyUserId == user.Id && link.IsActive))
            .OrderBy(user => user.LastName)
            .ThenBy(user => user.FirstName)
            .Select(user => new AssignableStudent(
                user.Id,
                user.LastName + " " + user.FirstName + " " + user.MiddleName,
                user.CourseBindUsers.Any(bind => bind.CourseId == courseId)))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AssignableStudent>> GetAssignableStudentsForPracticalAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()
            .Where(user => context.IdentityUserLinks.Any(link => link.LegacyUserId == user.Id && link.IsActive))
            .OrderBy(user => user.LastName)
            .ThenBy(user => user.FirstName)
            .Select(user => new AssignableStudent(
                user.Id,
                user.LastName + " " + user.FirstName + " " + user.MiddleName,
                user.PracticalBindUsers.Any(bind => bind.PracticalMaterialId == practicalId)))
            .ToListAsync(cancellationToken);
    }

    private IQueryable<AdminProfile> ProfilesQuery(Guid? legacyUserId = null)
    {
        var users = context.Users
            .AsNoTracking()
            .Where(user => legacyUserId == null || user.Id == legacyUserId);

        return users
            .GroupJoin(
                context.IdentityUserLinks.Where(link => link.IsActive),
                user => user.Id,
                link => link.LegacyUserId,
                (user, links) => new { user, link = links.FirstOrDefault() })
            .Select(item => new AdminProfile(
                item.user.Id,
                item.link == null ? null : item.link.IdentityUserId,
                item.user.Login,
                item.user.FirstName,
                item.user.LastName,
                item.user.MiddleName,
                item.link != null && item.link.IsActive));
    }

}


