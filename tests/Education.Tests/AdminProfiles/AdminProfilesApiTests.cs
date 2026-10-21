using System.Net;
using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.AdminProfiles;
using Education.Domain.Users;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Education.Web.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Tests.AdminProfiles;

public sealed class AdminProfilesApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public AdminProfilesApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Admin_CreatesProfile_WithIdentityUserIdAsId()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Admin);
        var identityUserId = Guid.NewGuid();

        var response = await client.PostAsJsonAsync(
            '/' + ApiRoutes.AdminProfiles.ProfilesList,
            new CreateAdminProfileRequest(
                identityUserId,
                "new.identity.user",
                "Identity",
                "User",
                "Linked"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var profile = await response.Content.ReadFromJsonAsync<AdminProfileResponse>();
        Assert.Equal(identityUserId, profile!.Id);

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var user = await dbContext.Users.FindAsync(profile.Id);

        Assert.NotNull(user);
        Assert.Equal(String.Empty, user!.Password);
        Assert.Equal(identityUserId, user.Id);
        Assert.True(user.IsActive);
    }

    [Theory]
    [InlineData(ProfileRole.Student)]
    [InlineData(ProfileRole.Teacher)]
    [InlineData(ProfileRole.Admin)]
    public async Task Admin_CreatesProfile_WithRequestedRole(ProfileRole role)
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Admin);

        var response = await client.PostAsJsonAsync(
            '/' + ApiRoutes.AdminProfiles.ProfilesList,
            new CreateAdminProfileRequest(
                Guid.NewGuid(),
                $"role.{role}.user",
                "Role",
                "User",
                String.Empty,
                role));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var profile = await response.Content.ReadFromJsonAsync<AdminProfileResponse>();

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var user = await dbContext.Users.FindAsync(profile!.Id);

        var expected = role switch
        {
            ProfileRole.Admin => RoleIds.Admin,
            ProfileRole.Teacher => RoleIds.Teacher,
            _ => RoleIds.Student,
        };
        Assert.Equal(expected, user!.RoleId);
    }

    [Fact]
    public async Task Admin_CreatesProfile_DefaultsToStudentRole()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Admin);

        var response = await client.PostAsJsonAsync(
            '/' + ApiRoutes.AdminProfiles.ProfilesList,
            new CreateAdminProfileRequest(Guid.NewGuid(), "default.role.user", "Default", "Role", String.Empty));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var profile = await response.Content.ReadFromJsonAsync<AdminProfileResponse>();

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var user = await dbContext.Users.FindAsync(profile!.Id);
        Assert.Equal(RoleIds.Student, user!.RoleId);
    }

    [Theory]
    [InlineData(EducationRoles.Teacher)]
    [InlineData(EducationRoles.Student)]
    public async Task NonAdmin_CannotUseAdminProfilesEndpoint(string role)
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(role);

        var response = await client.GetAsync('/' + ApiRoutes.AdminProfiles.ProfilesList);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task LocalRoleId_IsNotUsedForAdminAuthorization()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);

        var response = await client.GetAsync('/' + ApiRoutes.AdminProfiles.ProfilesList);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
