namespace Education.Application.Users;

/// <summary>
/// Сопоставляет пользователя внешнего сервиса идентификации с пользователем учебной системы.
/// </summary>
public interface IEducationUserResolver
{
    /// <summary>
    /// Возвращает идентификатор пользователя учебной системы по идентификатору пользователя identity-сервиса.
    /// </summary>
    Task<long> ResolveLegacyUserIdAsync(Guid identityUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает идентификатор пользователя учебной системы для текущего аутентифицированного пользователя.
    /// </summary>
    Task<long> ResolveCurrentLegacyUserIdAsync(CancellationToken cancellationToken = default);
}
