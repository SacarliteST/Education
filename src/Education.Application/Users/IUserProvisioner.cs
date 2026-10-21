namespace Education.Application.Users;

/// <summary>
/// Гарантирует, что у текущего аутентифицированного пользователя есть активный учебный профиль, и синхронизирует
/// ФИО и роль из токена. Создание — только для текущего пользователя и только если оно включено в настройках.
/// </summary>
public interface IUserProvisioner
{
    /// <summary>
    /// Возвращает идентификатор профиля текущего пользователя (он равен идентификатору в Identity).
    /// </summary>
    /// <exception cref="EducationUserLinkNotFoundException">
    /// Профиля нет и автосоздание выключено, либо профиль отключён администратором.
    /// </exception>
    Task<Guid> EnsureCurrentUserAsync(CancellationToken cancellationToken = default);
}
