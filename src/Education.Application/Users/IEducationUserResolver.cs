namespace Education.Application.Users;

public interface IEducationUserResolver
{
    Task<long> ResolveLegacyUserIdAsync(Guid identityUserId, CancellationToken cancellationToken = default);
    Task<long> ResolveCurrentLegacyUserIdAsync(CancellationToken cancellationToken = default);
}
