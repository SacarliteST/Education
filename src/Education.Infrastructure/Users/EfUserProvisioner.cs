using Education.Application.Identity;
using Education.Application.Users;
using Education.Domain.Users;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Education.Infrastructure.Users;

/// <summary>
/// Создаёт и синхронизирует учебный профиль текущего пользователя. Идентификатор профиля равен идентификатору в Identity,
/// поэтому гонку параллельных первых запросов закрывает первичный ключ: проигравший перечитывает строку победителя.
/// </summary>
internal sealed class EfUserProvisioner(
    EducationDbContext context,
    ICurrentUser currentUser,
    IConfiguration configuration,
    ILogger<EfUserProvisioner> logger) : IUserProvisioner
{
    /// <summary>Ключ настройки; по умолчанию автосоздание включено.</summary>
    public const string AutoProvisionKey = "Education:AutoProvisionProfiles";

    public async Task<Guid> EnsureCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUser.UserId;
        var user = await context.Users.FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);
        if (user is not null)
        {
            return await SyncAsync(user, cancellationToken);
        }

        if (!AutoProvisionEnabled())
        {
            throw new EducationUserLinkNotFoundException(userId);
        }

        return await CreateAsync(userId, cancellationToken);
    }

    private bool AutoProvisionEnabled() =>
        !bool.TryParse(configuration[AutoProvisionKey], out var enabled) || enabled;

    private async Task<Guid> SyncAsync(User user, CancellationToken cancellationToken)
    {
        if (!user.IsActive)
        {
            // Отключённый администратором профиль не восстанавливается автоматически.
            throw new EducationUserLinkNotFoundException(user.Id);
        }

        Guid? roleId = currentUser.Roles.Count == 0 ? null : ProfileClaims.ResolveRoleId(currentUser.Roles);
        var claims = ProfileClaims.FromCurrentUser(currentUser);
        var changed = claims.HasStructuredName
            ? user.SyncFromIdentity(claims.FirstName, claims.LastName, claims.MiddleName, roleId)
            : user.SyncFromIdentity(null, null, null, roleId);
        if (changed)
        {
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Профиль {UserId} синхронизирован с Identity (ФИО/роль).", user.Id);
        }

        return user.Id;
    }

    private async Task<Guid> CreateAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = ProfileClaims.FromCurrentUser(currentUser);
        var roleId = ProfileClaims.ResolveRoleId(currentUser.Roles);

        for (var attempt = 0; attempt < 2; attempt++)
        {
            var login = attempt == 0 ? profile.Login : $"{profile.Login}#{userId:N}"[..(profile.Login.Length + 9)];
            context.Users.Add(new User(userId, login, profile.FirstName, profile.LastName, profile.MiddleName, roleId));
            try
            {
                await context.SaveChangesAsync(cancellationToken);
                logger.LogInformation(
                    "Создан учебный профиль {UserId} (логин {Login}, роль {RoleId}).", userId, login, roleId);
                return userId;
            }
            catch (DbUpdateException exception) when (IsUniqueViolation(exception))
            {
                context.ChangeTracker.Clear();

                // Параллельный запрос того же пользователя успел раньше — используем его строку.
                var existing = await context.Users.AsNoTracking()
                    .Where(item => item.Id == userId)
                    .Select(item => new { item.IsActive })
                    .FirstOrDefaultAsync(cancellationToken);
                if (existing is not null)
                {
                    return existing.IsActive ? userId : throw new EducationUserLinkNotFoundException(userId);
                }

                // Иначе занят логин другим пользователем: не присваиваем чужую запись, создаём свою с суффиксом.
                if (attempt == 1)
                {
                    throw;
                }

                logger.LogWarning(
                    "Логин «{Login}» уже занят другим профилем; профиль {UserId} создаётся с уникальным логином.",
                    login, userId);
            }
        }

        throw new InvalidOperationException("Не удалось создать учебный профиль.");
    }

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
}
