using Education.Application.Identity;
using Education.Application.Users;

namespace Education.Tests;

public class UserResolverTests
{
    private static readonly Guid KnownIdentityUserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly Guid UnknownIdentityUserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    [Fact]
    public async Task KnownIdentityUserId_ResolvesToLegacyUserId()
    {
        var repository = new FakeUserProfileRepository();
        var resolver = new EducationUserResolver(
            new FakeCurrentUser(KnownIdentityUserId, true),
            repository);

        var legacyUserId = await resolver.ResolveLegacyUserIdAsync(KnownIdentityUserId);

        Assert.Equal(42, legacyUserId);
    }

    [Fact]
    public async Task UnknownIdentityUserId_ThrowsApplicationError()
    {
        var repository = new FakeUserProfileRepository();
        var resolver = new EducationUserResolver(
            new FakeCurrentUser(UnknownIdentityUserId, true),
            repository);

        var exception = await Assert.ThrowsAsync<EducationUserLinkNotFoundException>(
            () => resolver.ResolveLegacyUserIdAsync(UnknownIdentityUserId));

        Assert.Equal(UnknownIdentityUserId, exception.IdentityUserId);
    }

    [Fact]
    public async Task ExistingRelations_ContinueThroughLegacyUserId()
    {
        var repository = new FakeUserProfileRepository();
        var resolver = new EducationUserResolver(
            new FakeCurrentUser(KnownIdentityUserId, true),
            repository);

        var legacyUserId = await resolver.ResolveCurrentLegacyUserIdAsync();
        var relations = await repository.GetRelationsSnapshotAsync(legacyUserId);

        Assert.Equal(42, relations.LegacyUserId);
        Assert.Contains(1001, relations.OwnedCourseIds);
        Assert.Contains(2001, relations.AssignedCourseIds);
        Assert.Contains(3001, relations.AssignedPracticalIds);
        Assert.Contains(4001, relations.CaseFileIds);
        Assert.Contains(5001, relations.TestResultIds);
    }

    private sealed class FakeCurrentUser(Guid userId, bool isAuthenticated) : ICurrentUser
    {
        public Guid UserId { get; } = userId;
        public string? Email => "student@example.test";
        public string? Name => "Student Test";
        public IReadOnlySet<string> Roles { get; } = new HashSet<string>(["Student"]);
        public bool IsAuthenticated { get; } = isAuthenticated;
    }

    private sealed class FakeUserProfileRepository : IUserProfileRepository
    {
        private readonly Dictionary<Guid, long> links = new()
        {
            [KnownIdentityUserId] = 42,
        };

        private readonly Dictionary<long, UserRelationsSnapshot> relations = new()
        {
            [42] = new UserRelationsSnapshot(
                42,
                new HashSet<long> { 1001 },
                new HashSet<long> { 2001 },
                new HashSet<long> { 3001 },
                new HashSet<long> { 4001 },
                new HashSet<long> { 5001 }),
        };

        public Task<long?> FindLegacyUserIdByIdentityUserIdAsync(
            Guid identityUserId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(links.TryGetValue(identityUserId, out var legacyUserId)
                ? legacyUserId
                : (long?)null);
        }

        public Task<UserProfile?> FindProfileByLegacyUserIdAsync(
            long legacyUserId,
            CancellationToken cancellationToken = default)
        {
            if (legacyUserId != 42)
            {
                return Task.FromResult<UserProfile?>(null);
            }

            return Task.FromResult<UserProfile?>(new UserProfile(
                42,
                KnownIdentityUserId,
                "student",
                "Test",
                "Student",
                "User",
                true));
        }

        public Task<UserRelationsSnapshot> GetRelationsSnapshotAsync(
            long legacyUserId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(relations[legacyUserId]);
        }
    }
}
