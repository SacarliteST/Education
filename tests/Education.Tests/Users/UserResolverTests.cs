using Education.Application.Users;
using Education.Tests.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Tests.Users;

public class UserResolverTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public UserResolverTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task ExistingActiveProfile_ResolvesToItself_FromRealDatabase()
    {
        using var scope = factory.Services.CreateScope();
        var resolver = scope.ServiceProvider.GetRequiredService<IEducationUserResolver>();

        var userId = await resolver.ResolveLegacyUserIdAsync(TestAuthHandler.TestUserId);

        Assert.Equal(factory.Seed.TestUserId, userId);
        Assert.Equal(TestAuthHandler.TestUserId, userId);
    }

    [Fact]
    public async Task UnknownUserId_StillThrows_ForForeignIdentifiers()
    {
        using var scope = factory.Services.CreateScope();
        var resolver = scope.ServiceProvider.GetRequiredService<IEducationUserResolver>();

        await Assert.ThrowsAsync<EducationUserLinkNotFoundException>(() =>
            resolver.ResolveLegacyUserIdAsync(Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc")));
    }

    [Fact]
    public async Task ExistingRelations_ContinueThroughUserId_FromRealDatabase()
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
