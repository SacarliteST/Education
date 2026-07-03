namespace Education.Contracts.AdminProfiles;

/// <summary>
/// Запрос на создание локального учебного профиля и связи с identity-пользователем.
/// </summary>
/// <param name="IdentityUserId">Идентификатор пользователя во внешнем identity-сервисе.</param>
/// <param name="Login">Отображаемый логин или имя пользователя.</param>
/// <param name="FirstName">Имя пользователя.</param>
/// <param name="LastName">Фамилия пользователя.</param>
/// <param name="MiddleName">Отчество пользователя.</param>
public sealed record CreateAdminProfileRequest(
    Guid IdentityUserId,
    string Login,
    string FirstName,
    string LastName,
    string MiddleName);


