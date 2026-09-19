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

        var user = new User(
            command.Login,
            command.FirstName,
            command.LastName,
            command.MiddleName,
            command.RoleId,
            command.GroupName);
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
        user.ChangeGroup(command.GroupName);
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
            .Where(user => user.RoleId == RoleIds.Student
                && context.IdentityUserLinks.Any(link => link.LegacyUserId == user.Id && link.IsActive))
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
            .Where(user => user.RoleId == RoleIds.Student
                && context.IdentityUserLinks.Any(link => link.LegacyUserId == user.Id && link.IsActive))
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

    public Task<AssignableStudentsPage> GetAssignableStudentsPageForCourseAsync(
        Guid courseId,
        AssignableStudentsQuery query,
        CancellationToken cancellationToken = default)
    {
        var assignedIds = context.CourseBindUsers
            .Where(bind => bind.CourseId == courseId)
            .Select(bind => bind.UserId);
        return GetAssignableStudentsPageAsync(assignedIds, query, cancellationToken);
    }

    public Task<AssignableStudentsPage> GetAssignableStudentsPageForPracticalAsync(
        Guid practicalId,
        AssignableStudentsQuery query,
        CancellationToken cancellationToken = default)
    {
        var assignedIds = context.PracticalBindUsers
            .Where(bind => bind.PracticalMaterialId == practicalId)
            .Select(bind => bind.UserId);
        return GetAssignableStudentsPageAsync(assignedIds, query, cancellationToken);
    }

    public async Task ChangeCourseStudentsAsync(
        ChangeCourseStudentsCommand command,
        CancellationToken cancellationToken = default)
    {
        var addIds = command.Add.ToHashSet();
        var removeIds = command.Remove.ToHashSet();
        await EnsureLinkedUsersAsync(addIds, cancellationToken);

        var affected = await context.CourseBindUsers
            .Where(bind => bind.CourseId == command.CourseId
                && (addIds.Contains(bind.UserId) || removeIds.Contains(bind.UserId)))
            .ToListAsync(cancellationToken);

        context.CourseBindUsers.RemoveRange(affected.Where(bind => removeIds.Contains(bind.UserId)));

        var alreadyAssigned = affected.Select(bind => bind.UserId).ToHashSet();
        var newBinds = addIds
            .Where(userId => !alreadyAssigned.Contains(userId))
            .Select(userId => new CourseBindUser(command.CourseId, userId));

        await context.CourseBindUsers.AddRangeAsync(newBinds, cancellationToken);
        await SaveOrTranslateAsync(addIds, cancellationToken);
    }

    public async Task ChangePracticalStudentsAsync(
        ChangePracticalStudentsCommand command,
        CancellationToken cancellationToken = default)
    {
        var addIds = command.Add.ToHashSet();
        var removeIds = command.Remove.ToHashSet();
        await EnsureLinkedUsersAsync(addIds, cancellationToken);

        var affected = await context.PracticalBindUsers
            .Where(bind => bind.PracticalMaterialId == command.PracticalId
                && (addIds.Contains(bind.UserId) || removeIds.Contains(bind.UserId)))
            .ToListAsync(cancellationToken);

        context.PracticalBindUsers.RemoveRange(affected.Where(bind => removeIds.Contains(bind.UserId)));

        var alreadyAssigned = affected.Select(bind => bind.UserId).ToHashSet();
        var newBinds = addIds
            .Where(userId => !alreadyAssigned.Contains(userId))
            .Select(userId => new PracticalBindUser(command.PracticalId, userId));

        await context.PracticalBindUsers.AddRangeAsync(newBinds, cancellationToken);
        await SaveOrTranslateAsync(addIds, cancellationToken);
    }

    private async Task<AssignableStudentsPage> GetAssignableStudentsPageAsync(
        IQueryable<Guid> assignedIds,
        AssignableStudentsQuery query,
        CancellationToken cancellationToken)
    {
        var students = context.Users
            .AsNoTracking()
            .Where(user => user.RoleId == RoleIds.Student
                && context.IdentityUserLinks.Any(link => link.LegacyUserId == user.Id && link.IsActive));

        var assignedCount = await students.CountAsync(user => assignedIds.Contains(user.Id), cancellationToken);

        var filtered = students;
        if (!String.IsNullOrWhiteSpace(query.Search))
        {
            var pattern = "%" + EscapeLikePattern(query.Search.Trim()) + "%";
            filtered = filtered.Where(user =>
                EF.Functions.ILike(user.LastName + " " + user.FirstName + " " + user.MiddleName, pattern)
                || EF.Functions.ILike(user.Login, pattern)
                || (user.GroupName != null && EF.Functions.ILike(user.GroupName, pattern)));
        }

        if (!String.IsNullOrWhiteSpace(query.Group))
        {
            // ILike без подстановочных символов — точное совпадение без учёта регистра.
            var groupPattern = EscapeLikePattern(query.Group.Trim());
            filtered = filtered.Where(user => user.GroupName != null && EF.Functions.ILike(user.GroupName, groupPattern));
        }

        if (query.Assigned is { } assigned)
        {
            filtered = assigned
                ? filtered.Where(user => assignedIds.Contains(user.Id))
                : filtered.Where(user => !assignedIds.Contains(user.Id));
        }

        var totalCount = await filtered.CountAsync(cancellationToken);
        var items = await filtered
            .OrderBy(user => user.LastName)
            .ThenBy(user => user.FirstName)
            .ThenBy(user => user.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(user => new AssignableStudentEntry(
                user.Id,
                user.LastName + " " + user.FirstName + " " + user.MiddleName,
                user.Login,
                assignedIds.Contains(user.Id),
                user.GroupName))
            .ToListAsync(cancellationToken);

        return new AssignableStudentsPage(items, totalCount, assignedCount);
    }

    public async Task<IReadOnlyList<StudentGroup>> GetStudentGroupsAsync(CancellationToken cancellationToken = default)
    {
        var groups = await context.Users
            .AsNoTracking()
            .Where(user => user.RoleId == RoleIds.Student
                && user.GroupName != null
                && context.IdentityUserLinks.Any(link => link.LegacyUserId == user.Id && link.IsActive))
            .GroupBy(user => user.GroupName!)
            .Select(group => new { Name = group.Key, Count = group.Count() })
            .ToListAsync(cancellationToken);

        return groups
            .OrderBy(group => group.Name, StringComparer.OrdinalIgnoreCase)
            .Select(group => new StudentGroup(group.Name, group.Count))
            .ToList();
    }

    /// <summary>Экранирует служебные символы шаблона LIKE, чтобы поиск шёл по буквальной подстроке.</summary>
    private static string EscapeLikePattern(string value)
    {
        return value.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
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
                   link != null && link.IsActive,
                   user.GroupName);
    }

}


