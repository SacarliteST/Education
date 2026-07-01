namespace Education.Application.Users;

public sealed record UserProfile(
    long LegacyUserId,
    Guid? IdentityUserId,
    string Login,
    string FirstName,
    string LastName,
    string MiddleName,
    bool IsActive);
