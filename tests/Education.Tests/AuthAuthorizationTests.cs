using System.Net;
using Education.Tests.Auth;

namespace Education.Tests;

public class AuthAuthorizationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public AuthAuthorizationTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Student_CanAccessStudentEndpoint()
    {
        using var client = factory.CreateClient();
        client.AuthenticateAs("Student");

        var response = await client.GetAsync("/api/v1/student/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/v1/teacher/ping")]
    [InlineData("/api/v1/admin/ping")]
    public async Task Student_CannotAccessTeacherOrAdminEndpoints(string url)
    {
        using var client = factory.CreateClient();
        client.AuthenticateAs("Student");

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Teacher_CanAccessTeacherEndpoint()
    {
        using var client = factory.CreateClient();
        client.AuthenticateAs("Teacher");

        var response = await client.GetAsync("/api/v1/teacher/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Admin_CanAccessAdminEndpoint()
    {
        using var client = factory.CreateClient();
        client.AuthenticateAs("Admin");

        var response = await client.GetAsync("/api/v1/admin/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
