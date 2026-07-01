namespace Education.Application.Users;

public interface IUserProfileRepository
{
    Task<long?> FindLegacyUserIdByIdentityUserIdAsync(
        Guid identityUserId,
        CancellationToken cancellationToken = default);

    Task<UserProfile?> FindProfileByLegacyUserIdAsync(
        long legacyUserId,
        CancellationToken cancellationToken = default);

    Task<UserRelationsSnapshot> GetRelationsSnapshotAsync(
        long legacyUserId,
        CancellationToken cancellationToken = default);
}
