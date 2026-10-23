namespace Education.Application.Users;

/// <summary>Учебный профиль пользователя; <paramref name="Id"/> равен идентификатору пользователя в Identity.</summary>
public sealed record UserProfile(
    Guid Id,
    string Login,
    string FirstName,
    string LastName,
    string MiddleName,
    bool IsActive,
    Guid RoleId,
    string? GroupName);
