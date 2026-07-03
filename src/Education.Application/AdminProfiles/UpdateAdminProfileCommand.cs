namespace Education.Application.AdminProfiles;

/// <summary>
/// Команда обновления отображаемых данных локального учебного профиля.
/// </summary>
/// <param name="Login">Отображаемый логин или имя пользователя.</param>
/// <param name="FirstName">Имя пользователя.</param>
/// <param name="LastName">Фамилия пользователя.</param>
/// <param name="MiddleName">Отчество пользователя.</param>
public sealed record UpdateAdminProfileCommand(string Login, string FirstName, string LastName, string MiddleName);

