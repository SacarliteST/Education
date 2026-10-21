using Education.Application.Identity;

namespace Education.Application.Users;

/// <summary>
/// Разрешает идентификатор учебного пользователя. Идентификатор равен идентификатору в Identity, поэтому сопоставления
/// не нужно — проверяется, что активный профиль существует (для текущего пользователя он при необходимости создаётся).
/// </summary>
public sealed class EducationUserResolver(
    ICurrentUser currentUser,
    IUserProfileRepository userProfileRepository,
    IUserProvisioner userProvisioner) : IEducationUserResolver
{
    private Guid? currentUserId;

    public async Task<Guid> ResolveLegacyUserIdAsync(Guid identityUserId, CancellationToken cancellationToken = default)
    {
        // Чужие идентификаторы (серверные и модульные потоки) — только по существующему активному профилю.
        return await userProfileRepository.IsActiveAsync(identityUserId, cancellationToken)
            ? identityUserId
            : throw new EducationUserLinkNotFoundException(identityUserId);
    }

    public async Task<Guid> ResolveCurrentLegacyUserIdAsync(CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId == Guid.Empty)
        {
            throw new UnauthenticatedEducationUserException();
        }

        // Профиль проверяется и синхронизируется один раз на запрос.
        currentUserId ??= await userProvisioner.EnsureCurrentUserAsync(cancellationToken);
        return currentUserId.Value;
    }
}
