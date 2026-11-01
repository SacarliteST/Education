namespace Education.Application.Users;

/// <summary>Профиль, собранный из данных токена.</summary>
/// <param name="Login">Логин (email).</param>
/// <param name="FirstName">Имя.</param>
/// <param name="LastName">Фамилия.</param>
/// <param name="MiddleName">Отчество.</param>
/// <param name="HasStructuredName">Имя пришло структурированно (given_name/family_name), а не разобрано из строки.</param>
/// <param name="GroupName">Учебная группа из claim <c>group_name</c>; <see langword="null"/>, если не назначалась.</param>
public sealed record ProfileFromClaims(
    string Login,
    string FirstName,
    string LastName,
    string MiddleName,
    bool HasStructuredName,
    string? GroupName = null);
