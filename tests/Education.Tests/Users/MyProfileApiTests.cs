using System.Net;
using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.Profile;
using Education.Domain.Users;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Education.Web.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Tests.Users;

public sealed class MyProfileApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public MyProfileApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Student_SeesOwnProfile_WithGroup()
    {
        var id = Guid.NewGuid();
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
            db.Users.Add(new User(id, $"cab{id:N}@x.test", "Анна", "Смирнова", "-", RoleIds.Student, "ИС-21"));
            await db.SaveChangesAsync();
        }

        var client = factory.CreateClient();
        client.AuthenticateAsIdentity(id, [EducationRoles.Student], $"cab{id:N}@x.test");

        var response = await client.GetAsync('/' + ApiRoutes.Profile.Me);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var profile = (await response.Content.ReadFromJsonAsync<MyEducationProfileResponse>())!;
        Assert.Equal(id, profile.Id);
        Assert.Equal("ИС-21", profile.Group);
        Assert.Equal("Student", profile.Role);
        Assert.Equal("Смирнова", profile.LastName);
    }

    [Fact]
    public async Task NewUser_GetsAutoCreatedProfile_WithoutGroup()
    {
        var id = Guid.NewGuid();
        var client = factory.CreateClient();
        client.AuthenticateAsIdentity(id, [EducationRoles.Teacher], $"new{id:N}@x.test", "Петр Иванов", "Петр", "Иванов");

        var response = await client.GetAsync('/' + ApiRoutes.Profile.Me);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var profile = (await response.Content.ReadFromJsonAsync<MyEducationProfileResponse>())!;
        Assert.Equal("Teacher", profile.Role);
        Assert.Null(profile.Group);
    }

    [Fact]
    public async Task Unauthenticated_Gets401()
    {
        var response = await factory.CreateClient().GetAsync('/' + ApiRoutes.Profile.Me);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
