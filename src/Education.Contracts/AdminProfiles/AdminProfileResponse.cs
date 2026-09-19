namespace Education.Contracts.AdminProfiles;

/// <summary>
/// Локальный учебный профиль пользователя.
/// </summary>
/// <param name="LegacyUserId">Идентификатор пользователя в учебной системе.</param>
/// <param name="IdentityUserId">Идентификатор пользователя во внешнем identity-сервисе.</param>
/// <param name="Login">Отображаемый логин или имя пользователя.</param>
/// <param name="FirstName">Имя пользователя.</param>
/// <param name="LastName">Фамилия пользователя.</param>
/// <param name="MiddleName">Отчество пользователя.</param>
/// <param name="IsActive">Признак активной связи с identity-сервисом.</param>
/// <param name="Group">Учебная группа или <see langword="null"/>, если не задана.</param>
public sealed record AdminProfileResponse(
    Guid LegacyUserId,
    Guid? IdentityUserId,
    string Login,
    string FirstName,
    string LastName,
    string MiddleName,
    bool IsActive,
    string? Group = null);


