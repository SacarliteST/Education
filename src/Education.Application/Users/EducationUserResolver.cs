using Education.Application.Identity;

namespace Education.Application.Users;

/// <summary>
/// Разрешает идентификатор учебного пользователя. Идентификатор равен идентификатору в Identity, поэтому сопоставления
/// не нужно — проверяется, что активный профиль существует (для текущего пользователя он при необходимости создаётся).
/// </summary>
public sealed class EducationUserResolver : IEducationUserResolver
{
    private readonly ICurrentUser currentUser;
    private readonly IUserProfileRepository userProfileRepository;
    private readonly IUserProvisioner userProvisioner;

    /// <summary>Создаёт экземпляр класса <see cref="EducationUserResolver"/>.</summary>
    /// <param name="currentUser">Текущий пользователь запроса.</param>
    /// <param name="userProfileRepository">Хранилище профилей пользователей.</param>
    /// <param name="userProvisioner">Автосоздание и синхронизация учебного профиля.</param>
    public EducationUserResolver(
        ICurrentUser currentUser,
        IUserProfileRepository userProfileRepository,
        IUserProvisioner userProvisioner)
    {
        this.currentUser = currentUser;
        this.userProfileRepository = userProfileRepository;
        this.userProvisioner = userProvisioner;
    }

    private Guid? currentUserId;

    /// <inheritdoc />
    public async Task<Guid> ResolveLegacyUserIdAsync(Guid identityUserId, CancellationToken cancellationToken = default)
    {
        // Чужие идентификаторы (серверные и модульные потоки) — только по существующему активному профилю.
        return await userProfileRepository.IsActiveAsync(identityUserId, cancellationToken)
            ? identityUserId
            : throw new EducationUserLinkNotFoundException(identityUserId);
    }

    /// <inheritdoc />
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
