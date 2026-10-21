namespace Education.Contracts.AdminProfiles;

/// <summary>
/// Запрос на ручное создание (или восстановление отключённого) профиля. Обычно профиль создаётся сам при первом
/// обращении пользователя; ручное создание нужно, когда автосоздание выключено.
/// </summary>
/// <param name="UserId">Идентификатор пользователя в Identity; он же станет идентификатором профиля.</param>
/// <param name="Login">Отображаемый логин (email).</param>
/// <param name="FirstName">Имя.</param>
/// <param name="LastName">Фамилия.</param>
/// <param name="MiddleName">Отчество.</param>
/// <param name="Role">Роль профиля.</param>
/// <param name="Group">Учебная группа (до 50 символов) или <c>null</c>.</param>
public sealed record CreateAdminProfileRequest(
    Guid UserId,
    string Login,
    string FirstName,
    string LastName,
    string MiddleName,
    ProfileRole Role = ProfileRole.Student,
    string? Group = null);
