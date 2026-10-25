using System.Net;
using Education.Contracts;
using Education.Web.Identity;

namespace Education.Tests.Auth;

/// <summary>Политики доступа по ролям: кто какие маршруты видит.</summary>
public sealed class AuthorizationPolicyTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public AuthorizationPolicyTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Theory]
    [InlineData("/api/v1/auth/me")]
    [InlineData("/api/v1/student/ping")]
    [InlineData("/api/v1/courses/student")]
    public async Task WithoutToken_Returns401(string url)
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData(EducationRoles.Student, "/api/v1/student/ping", HttpStatusCode.OK)]
    [InlineData(EducationRoles.Student, "/api/v1/teacher/ping", HttpStatusCode.Forbidden)]
    [InlineData(EducationRoles.Student, "/api/v1/admin/ping", HttpStatusCode.Forbidden)]
    [InlineData(EducationRoles.Teacher, "/api/v1/teacher/ping", HttpStatusCode.OK)]
    [InlineData(EducationRoles.Teacher, "/api/v1/admin/ping", HttpStatusCode.Forbidden)]
    [InlineData(EducationRoles.Admin, "/api/v1/admin/ping", HttpStatusCode.OK)]
    public async Task RoleMatrix_BearerToken(string role, string url, HttpStatusCode expected)
    {
        using var client = factory.CreateClient();
        client.AuthenticateWithBearer(role);

        var response = await client.GetAsync(url);

        Assert.Equal(expected, response.StatusCode);
    }

    [Fact]
    public async Task Student_CannotListAdminProfiles()
    {
        using var client = factory.CreateClient();
        client.AuthenticateWithBearer(EducationRoles.Student);

        var response = await client.GetAsync('/' + ApiRoutes.AdminProfiles.ProfilesList);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
