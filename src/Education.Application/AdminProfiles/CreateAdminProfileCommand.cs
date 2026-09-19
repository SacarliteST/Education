namespace Education.Application.AdminProfiles;

/// <summary>
/// Команда создания локального учебного профиля и связи с пользователем identity-сервиса.
/// </summary>
/// <param name="IdentityUserId">Идентификатор пользователя во внешнем identity-сервисе.</param>
/// <param name="Login">Отображаемый логин или имя пользователя.</param>
/// <param name="FirstName">Имя пользователя.</param>
/// <param name="LastName">Фамилия пользователя.</param>
/// <param name="MiddleName">Отчество пользователя.</param>
/// <param name="RoleId">Идентификатор роли профиля (см. <c>RoleIds</c>).</param>
public sealed record CreateAdminProfileCommand(
    Guid IdentityUserId,
    string Login,
    string FirstName,
    string LastName,
    string MiddleName,
    Guid RoleId,
    string? GroupName = null);

