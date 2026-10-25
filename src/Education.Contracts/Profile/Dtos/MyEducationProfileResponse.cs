namespace Education.Contracts.Profile;

/// <summary>
/// Учебный профиль текущего пользователя для личного кабинета.
/// </summary>
/// <param name="Id">Идентификатор профиля; равен идентификатору пользователя в Identity.</param>
/// <param name="Login">Логин (email).</param>
/// <param name="FirstName">Имя (копия данных Identity).</param>
/// <param name="LastName">Фамилия (копия данных Identity).</param>
/// <param name="MiddleName">Отчество (копия данных Identity).</param>
/// <param name="Role">Роль в учебной системе: <c>Admin</c>, <c>Teacher</c> или <c>Student</c>.</param>
/// <param name="Group">Учебная группа или <c>null</c>; назначает администратор, пользователь её не меняет.</param>
public sealed record MyEducationProfileResponse(
    Guid Id,
    string Login,
    string FirstName,
    string LastName,
    string MiddleName,
    string Role,
    string? Group);
