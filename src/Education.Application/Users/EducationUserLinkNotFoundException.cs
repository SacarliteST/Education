namespace Education.Application.Users;

public sealed class EducationUserLinkNotFoundException(Guid identityUserId)
    : InvalidOperationException($"Education user link was not found for identity user '{identityUserId}'.")
{
    public Guid IdentityUserId { get; } = identityUserId;
}
