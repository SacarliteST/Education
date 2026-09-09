using Education.Application.AdminProfiles;
using Education.Domain.Courses;
using Education.Domain.Practicals;
using Education.Domain.Users;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.AdminProfiles;

internal sealed class EfAdminProfilesRepository(EducationDbContext context) : IAdminProfilesRepository
{
    public async Task<IReadOnlyList<AdminProfile>> GetProfilesAsync(CancellationToken cancellationToken = default)
    {
        var profiles = await ProfilesQuery().ToListAsync(cancellationToken);

        return profiles
            .OrderBy(profile => profile.LastName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(profile => profile.FirstName, StringComparer.OrdinalIgnoreCase)
            .ToList();
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

    public async Task SetCourseStudentsAsync(
        SetCourseStudentsCommand command,
        CancellationToken cancellationToken = default)
    {
        var desiredIds = command.UserIds.ToHashSet();
        await EnsureLinkedUsersAsync(desiredIds, cancellationToken);

        var currentBinds = await context.CourseBindUsers
            .Where(bind => bind.CourseId == command.CourseId)
            .ToListAsync(cancellationToken);

        context.CourseBindUsers.RemoveRange(currentBinds.Where(bind => !desiredIds.Contains(bind.UserId)));

        var currentIds = currentBinds.Select(bind => bind.UserId).ToHashSet();
        var newBinds = desiredIds
            .Where(userId => !currentIds.Contains(userId))
            .Select(userId => new CourseBindUser(command.CourseId, userId));

        await context.CourseBindUsers.AddRangeAsync(newBinds, cancellationToken);
        await SaveOrTranslateAsync(desiredIds, cancellationToken);
    }

    public async Task SetPracticalStudentsAsync(
        SetPracticalStudentsCommand command,
        CancellationToken cancellationToken = default)
    {
        var desiredIds = command.UserIds.ToHashSet();
        await EnsureLinkedUsersAsync(desiredIds, cancellationToken);

        var currentBinds = await context.PracticalBindUsers
            .Where(bind => bind.PracticalMaterialId == command.PracticalId)
            .ToListAsync(cancellationToken);

        context.PracticalBindUsers.RemoveRange(currentBinds.Where(bind => !desiredIds.Contains(bind.UserId)));

        var currentIds = currentBinds.Select(bind => bind.UserId).ToHashSet();
        var newBinds = desiredIds
            .Where(userId => !currentIds.Contains(userId))
            .Select(userId => new PracticalBindUser(command.PracticalId, userId));

        await context.PracticalBindUsers.AddRangeAsync(newBinds, cancellationToken);
        await SaveOrTranslateAsync(desiredIds, cancellationToken);
    }

    /// <summary>Каждый id должен быть учебным профилем с активной связью с identity-сервисом.</summary>
    private async Task EnsureLinkedUsersAsync(
        IReadOnlyCollection<Guid> desiredIds,
        CancellationToken cancellationToken)
    {
        if (desiredIds.Count == 0)
        {
            return;
        }

        var linkedIds = await context.Users
            .AsNoTracking()
            .Where(user => desiredIds.Contains(user.Id)
                && context.IdentityUserLinks.Any(link => link.LegacyUserId == user.Id && link.IsActive))
            .Select(user => user.Id)
            .ToHashSetAsync(cancellationToken);

        var unknown = desiredIds.Where(id => !linkedIds.Contains(id)).ToArray();
        if (unknown.Length > 0)
        {
            throw new UnknownStudentsException(unknown);
        }
    }

    /// <summary>Страховка на гонку: если строку пользователя удалили между проверкой и записью — переводим FK в 400.</summary>
    private async Task SaveOrTranslateAsync(IReadOnlyCollection<Guid> desiredIds, CancellationToken cancellationToken)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            context.ChangeTracker.Clear();
            throw new UnknownStudentsException(desiredIds);
        }
    }

    private IQueryable<AdminProfile> ProfilesQuery(Guid? legacyUserId = null)
    {
        return from user in context.Users.AsNoTracking()
               where legacyUserId == null || user.Id == legacyUserId
               from link in context.IdentityUserLinks
                   .Where(item => item.IsActive && item.LegacyUserId == user.Id)
                   .DefaultIfEmpty()
               select new AdminProfile(
                   user.Id,
                   link != null ? (Guid?)link.IdentityUserId : null,
                   user.Login,
                   user.FirstName,
                   user.LastName,
                   user.MiddleName,
                   link != null && link.IsActive);
    }

}


