namespace Education.Application.AdminProfiles;

/// <summary>Профиль пользователя в админке. <paramref name="LegacyUserId"/> равен идентификатору в Identity.</summary>
public sealed record AdminProfile(
    Guid LegacyUserId,
    string Login,
    string FirstName,
    string LastName,
    string MiddleName,
    bool IsActive,
    string? GroupName = null);
