using System.Net;
using System.Net.Http.Json;
using Education.Application.Users;
using Education.Contracts;
using Education.Domain.Users;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Education.Web.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Tests.Users;

public sealed class ProfileProvisioningApiTests : IClassFixture<TestWebApplicationFactory>
{
    private const string MeRoute = "/api/v1/auth/me";

    private readonly TestWebApplicationFactory factory;

    public ProfileProvisioningApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task NewStudent_FirstRequest_CreatesProfileFromTokenClaims()
    {
        var id = Guid.NewGuid();
        var client = factory.CreateClient();
        client.AuthenticateAsIdentity(
            id, [EducationRoles.Student], $"s{id:N}@x.test", "Анна Смирнова", "Анна", "Смирнова", "Игоревна");

        var response = await client.GetAsync(MeRoute);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await FindAsync(id);
        Assert.NotNull(user);
        Assert.Equal(RoleIds.Student, user!.RoleId);
        Assert.Null(user.GroupName);
        Assert.True(user.IsActive);
        Assert.Equal($"s{id:N}@x.test", user.Login);
        Assert.Equal("Анна", user.FirstName);
        Assert.Equal("Смирнова", user.LastName);
        Assert.Equal("Игоревна", user.MiddleName);
    }

    [Fact]
    public async Task RepeatedRequests_DoNotCreateDuplicates()
    {
        var id = Guid.NewGuid();
        var client = factory.CreateClient();
        client.AuthenticateAsIdentity(id, [EducationRoles.Student], $"r{id:N}@x.test", "Ivan Petrov");

        await client.GetAsync(MeRoute);
        await client.GetAsync(MeRoute);

        Assert.Equal(1, await CountAsync(id));
    }

    [Theory]
    [InlineData(EducationRoles.Teacher)]
    [InlineData(EducationRoles.Admin)]
    public async Task TeacherAndAdmin_GetMatchingRole(string role)
    {
        var id = Guid.NewGuid();
        var client = factory.CreateClient();
        client.AuthenticateAsIdentity(id, [role], $"{role}{id:N}@x.test", "Role User");

        var response = await client.GetAsync(MeRoute);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var expected = role == EducationRoles.Admin ? RoleIds.Admin : RoleIds.Teacher;
        Assert.Equal(expected, (await FindAsync(id))!.RoleId);
    }

    [Fact]
    public async Task AdminWithTeacherRole_PrefersAdmin()
    {
        var id = Guid.NewGuid();
        var client = factory.CreateClient();
        client.AuthenticateAsIdentity(id, [EducationRoles.Teacher, EducationRoles.Admin], $"m{id:N}@x.test", "Multi Role");

        await client.GetAsync(MeRoute);

        Assert.Equal(RoleIds.Admin, (await FindAsync(id))!.RoleId);
    }

    [Fact]
    public async Task TwentyParallelFirstRequests_CreateExactlyOneProfile()
    {
        var id = Guid.NewGuid();
        var email = $"p{id:N}@x.test";

        var responses = await Task.WhenAll(Enumerable.Range(0, 20).Select(async _ =>
        {
            var client = factory.CreateClient();
            client.AuthenticateAsIdentity(id, [EducationRoles.Student], email, "Para Llel", "Para", "Llel");
            return await client.GetAsync(MeRoute);
        }));

        Assert.All(responses, response => Assert.Equal(HttpStatusCode.OK, response.StatusCode));
        Assert.Equal(1, await CountAsync(id));
    }

    [Fact]
    public async Task LoginTakenByAnotherUser_CreatesNewProfileWithUniqueLogin_AndKeepsTheOldOne()
    {
        var oldId = Guid.NewGuid();
        var newId = Guid.NewGuid();
        var email = $"taken{oldId:N}@x.test";
        await AddUserAsync(new User(oldId, email, "Old", "Owner", "-", RoleIds.Student));

        var client = factory.CreateClient();
        client.AuthenticateAsIdentity(newId, [EducationRoles.Student], email, "New Person", "New", "Person");
        var response = await client.GetAsync(MeRoute);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var created = await FindAsync(newId);
        Assert.Equal($"{email}#{newId:N}"[..(email.Length + 9)], created!.Login);
        var old = await FindAsync(oldId);
        Assert.Equal(email, old!.Login);
        Assert.Equal("Old", old.FirstName);
    }

    [Fact]
    public async Task DeactivatedUser_IsNotRecreated_AndGetsForbiddenOnProfileEndpoints()
    {
        var id = Guid.NewGuid();
        var user = new User(id, $"off{id:N}@x.test", "Off", "User", "-", RoleIds.Student);
        user.Deactivate();
        await AddUserAsync(user);
        var client = factory.CreateClient();
        client.AuthenticateAsIdentity(id, [EducationRoles.Student], user.Login, "Off User", "Off", "User");

        var forbidden = await client.GetAsync('/' + ApiRoutes.Courses.StudentCourses);
        var me = await client.GetAsync(MeRoute);

        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
        Assert.False((await FindAsync(id))!.IsActive);
        Assert.Equal(1, await CountAsync(id));
    }

    [Fact]
    public async Task AutoProvisionDisabled_BehavesAsBefore_NoProfileIsCreated()
    {
        await using var disabled = factory.WithWebHostBuilder(builder =>
            builder.UseSetting("Education:AutoProvisionProfiles", "false"));
        var id = Guid.NewGuid();
        var client = disabled.CreateClient();
        client.AuthenticateAsIdentity(id, [EducationRoles.Student], $"d{id:N}@x.test", "No Profile");

        var forbidden = await client.GetAsync('/' + ApiRoutes.Courses.StudentCourses);

        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
        Assert.Equal(0, await CountAsync(id));
    }

    [Fact]
    public async Task Unauthenticated_IsStillRejected_AndCreatesNothing()
    {
        var before = await TotalAsync();
        var client = factory.CreateClient();

        var response = await client.GetAsync(MeRoute);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(before, await TotalAsync());
    }

    [Fact]
    public async Task StructuredClaims_SyncNamesAndRole_ButKeepGroupAndLogin()
    {
        var id = Guid.NewGuid();
        var user = new User(id, $"sync{id:N}@x.test", "Old", "Name", "-", RoleIds.Student, "ИС-21");
        await AddUserAsync(user);
        var client = factory.CreateClient();
        client.AuthenticateAsIdentity(
            id, [EducationRoles.Teacher], "other@x.test", "Fio New", "Fio", "New", "Mid");

        await client.GetAsync(MeRoute);

        var synced = (await FindAsync(id))!;
        Assert.Equal("Fio", synced.FirstName);
        Assert.Equal("New", synced.LastName);
        Assert.Equal("Mid", synced.MiddleName);
        Assert.Equal(RoleIds.Teacher, synced.RoleId);
        Assert.Equal("ИС-21", synced.GroupName);
        Assert.Equal($"sync{id:N}@x.test", synced.Login);
    }

    [Fact]
    public async Task NoStructuredClaims_DoNotOverwriteNames_ButSyncRole()
    {
        var id = Guid.NewGuid();
        await AddUserAsync(new User(id, $"keep{id:N}@x.test", "Kept", "Names", "-", RoleIds.Student));
        var client = factory.CreateClient();
        client.AuthenticateAsIdentity(id, [EducationRoles.Admin], $"keep{id:N}@x.test", "Totally Different");

        await client.GetAsync(MeRoute);

        var user = (await FindAsync(id))!;
        Assert.Equal("Kept", user.FirstName);
        Assert.Equal("Names", user.LastName);
        Assert.Equal(RoleIds.Admin, user.RoleId);
    }

    [Fact]
    public async Task AdminCreate_ForExistingActiveProfile_Conflicts_ForDeactivated_Restores()
    {
        var activeId = Guid.NewGuid();
        var offId = Guid.NewGuid();
        var off = new User(offId, $"off2{offId:N}@x.test", "Off", "Two", "-", RoleIds.Student);
        off.Deactivate();
        await AddUserAsync(new User(activeId, $"act{activeId:N}@x.test", "A", "B", "-", RoleIds.Student));
        await AddUserAsync(off);
        var admin = factory.CreateClient();
        admin.AuthenticateAs(EducationRoles.Admin);

        var conflict = await admin.PostAsJsonAsync(
            '/' + ApiRoutes.AdminProfiles.ProfilesList,
            new Education.Contracts.AdminProfiles.CreateAdminProfileRequest(activeId, "x", "A", "B", "-"));
        var restored = await admin.PostAsJsonAsync(
            '/' + ApiRoutes.AdminProfiles.ProfilesList,
            new Education.Contracts.AdminProfiles.CreateAdminProfileRequest(offId, "x2", "Off", "Two", "-"));

        Assert.Equal(HttpStatusCode.Conflict, conflict.StatusCode);
        Assert.Equal(HttpStatusCode.Created, restored.StatusCode);
        Assert.True((await FindAsync(offId))!.IsActive);
    }

    private async Task<User?> FindAsync(Guid id)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        return await db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == id);
    }

    private async Task<int> CountAsync(Guid id)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        return await db.Users.CountAsync(user => user.Id == id);
    }

    private async Task<int> TotalAsync()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        return await db.Users.CountAsync();
    }

    private async Task AddUserAsync(User user)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }
}
