namespace Education.Application.Users;

/// <summary>
/// Читает профиль пользователя и связи между пользователями учебной системы и identity-сервиса.
/// </summary>
public interface IUserProfileRepository
{
    /// <summary>
    /// Ищет идентификатор пользователя учебной системы по идентификатору пользователя identity-сервиса.
    /// </summary>
    Task<Guid?> FindLegacyUserIdByIdentityUserIdAsync(
        Guid identityUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ищет профиль пользователя учебной системы по его идентификатору.
    /// </summary>
    Task<UserProfile?> FindProfileByLegacyUserIdAsync(
        Guid legacyUserId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает снимок связей пользователя с курсами, практическими материалами и результатами.
    /// </summary>
    Task<UserRelationsSnapshot> GetRelationsSnapshotAsync(
        Guid legacyUserId,
        CancellationToken cancellationToken = default);
}

