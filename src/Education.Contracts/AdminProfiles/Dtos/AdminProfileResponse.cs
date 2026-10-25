namespace Education.Contracts.AdminProfiles;

/// <summary>
/// Профиль пользователя в административном контуре.
/// </summary>
/// <param name="Id">Идентификатор профиля; равен идентификатору пользователя в Identity.</param>
/// <param name="Login">Отображаемый логин (email).</param>
/// <param name="FirstName">Имя (копия данных Identity).</param>
/// <param name="LastName">Фамилия (копия данных Identity).</param>
/// <param name="MiddleName">Отчество (копия данных Identity).</param>
/// <param name="IsActive">Профиль активен; отключённый администратором доступа не имеет.</param>
/// <param name="Group">Учебная группа или <c>null</c>.</param>
public sealed record AdminProfileResponse(
    Guid Id,
    string Login,
    string FirstName,
    string LastName,
    string MiddleName,
    bool IsActive,
    string? Group = null);
