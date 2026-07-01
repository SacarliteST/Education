using System.Net;
using Education.Tests.Auth;

namespace Education.Tests;

public class AuthAuthorizationTests
{
    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Student_CanAccessStudentEndpoint()
    {
        await using var factory = new TestWebApplicationFactory();
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
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        client.AuthenticateAs("Student");

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Teacher_CanAccessTeacherEndpoint()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        client.AuthenticateAs("Teacher");

        var response = await client.GetAsync("/api/v1/teacher/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Admin_CanAccessAdminEndpoint()
    {
        await using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();
        client.AuthenticateAs("Admin");

        var response = await client.GetAsync("/api/v1/admin/ping");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
