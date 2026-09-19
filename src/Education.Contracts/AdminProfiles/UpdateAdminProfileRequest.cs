namespace Education.Contracts.AdminProfiles;

/// <summary>
/// Запрос на обновление отображаемых данных локального учебного профиля.
/// </summary>
/// <param name="Login">Отображаемый логин или имя пользователя.</param>
/// <param name="FirstName">Имя пользователя.</param>
/// <param name="LastName">Фамилия пользователя.</param>
/// <param name="MiddleName">Отчество пользователя.</param>
/// <param name="Group">Учебная группа (до 50 символов); пусто или отсутствие значения снимает группу.</param>
public sealed record UpdateAdminProfileRequest(
    string Login,
    string FirstName,
    string LastName,
    string MiddleName,
    string? Group = null);


