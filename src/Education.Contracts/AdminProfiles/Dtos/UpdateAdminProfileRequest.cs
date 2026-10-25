namespace Education.Contracts.AdminProfiles;

/// <summary>
/// Запрос на обновление учебных данных профиля. ФИО и роль принадлежат Identity и здесь не правятся.
/// </summary>
/// <param name="Group">Учебная группа (до 50 символов); пусто или отсутствие значения снимает группу.</param>
public sealed record UpdateAdminProfileRequest(string? Group = null);
