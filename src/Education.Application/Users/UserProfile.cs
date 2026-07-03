namespace Education.Application.Users;

public sealed record UserProfile(
    Guid LegacyUserId,
    Guid? IdentityUserId,
    string Login,
    string FirstName,
    string LastName,
    string MiddleName,
    bool IsActive);

