namespace Education.Application.AdminProfiles;

/// <summary>Ручное создание (или восстановление) профиля. <paramref name="UserId"/> — идентификатор в Identity.</summary>
public sealed record CreateAdminProfileCommand(
    Guid UserId,
    string Login,
    string FirstName,
    string LastName,
    string MiddleName,
    Guid RoleId,
    string? GroupName = null);
