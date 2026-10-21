namespace Education.Application.Users;

/// <summary>
/// Читает учебный профиль пользователя. Идентификатор профиля равен идентификатору пользователя в Identity.
/// </summary>
public interface IUserProfileRepository
{
    /// <summary>
    /// Проверяет, что активный профиль с таким идентификатором существует.
    /// </summary>
    Task<bool> IsActiveAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ищет профиль пользователя учебной системы по его идентификатору.
    /// </summary>
    Task<UserProfile?> FindProfileAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает снимок связей пользователя с курсами, практическими материалами и результатами.
    /// </summary>
    Task<UserRelationsSnapshot> GetRelationsSnapshotAsync(Guid userId, CancellationToken cancellationToken = default);
}
