using Education.Application.Identity;

namespace Education.Application.Users;

public sealed class EducationUserResolver(ICurrentUser currentUser, IUserProfileRepository userProfileRepository)
    : IEducationUserResolver
{
    public async Task<Guid> ResolveLegacyUserIdAsync(Guid identityUserId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userProfileRepository.FindLegacyUserIdByIdentityUserIdAsync(
            identityUserId,
            cancellationToken);

        return legacyUserId ?? throw new EducationUserLinkNotFoundException(identityUserId);
    }

    public Task<Guid> ResolveCurrentLegacyUserIdAsync(CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId == Guid.Empty)
        {
            throw new UnauthenticatedEducationUserException();
        }

        return ResolveLegacyUserIdAsync(currentUser.UserId, cancellationToken);
    }
}

