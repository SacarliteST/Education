using Education.Application.Users;
using Education.Tests.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Tests;

public class UserResolverTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public UserResolverTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task KnownIdentityUserId_ResolvesToLegacyUserId_FromRealDatabase()
    {
        using var scope = factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();

        var legacyUserId = await repository.FindLegacyUserIdByIdentityUserIdAsync(TestAuthHandler.TestUserId);

        Assert.Equal(factory.Seed.TestUserId, legacyUserId);
    }

    [Fact]
    public async Task UnknownIdentityUserId_ReturnsNoLegacyUser_FromRealDatabase()
    {
        using var scope = factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();

        var legacyUserId = await repository.FindLegacyUserIdByIdentityUserIdAsync(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"));

        Assert.Null(legacyUserId);
    }

    [Fact]
    public async Task ExistingRelations_ContinueThroughLegacyUserId_FromRealDatabase()
    {
        using var scope = factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserProfileRepository>();

        var relations = await repository.GetRelationsSnapshotAsync(factory.Seed.TestUserId);

        Assert.Equal(factory.Seed.TestUserId, relations.LegacyUserId);
        Assert.Contains(factory.Seed.OwnCourseId, relations.OwnedCourseIds);
        Assert.Contains(factory.Seed.OwnCourseId, relations.AssignedCourseIds);
        Assert.Contains(factory.Seed.AssignedStartPracticalId, relations.AssignedPracticalIds);
        Assert.Contains(factory.Seed.OtherTeacherCaseFileId, relations.CaseFileIds);
        Assert.Contains(factory.Seed.CompletedLimitedResultId, relations.TestResultIds);
    }
}
