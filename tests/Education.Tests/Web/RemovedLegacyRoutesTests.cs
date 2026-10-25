using System.Net;
using Education.Tests.Auth;
using Education.Web.Identity;

namespace Education.Tests.Web;

/// <summary>Старые маршруты прежнего MVC-хоста не должны вернуться.</summary>
public sealed class RemovedLegacyRoutesTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public RemovedLegacyRoutesTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Theory]
    [InlineData("GET", "/api/Auth/IsSignedIn")]
    [InlineData("GET", "/api/Student/GetCourses")]
    [InlineData("GET", "/api/Teacher/GetCourses")]
    [InlineData("GET", "/api/Shared/GetModules?courseId=1")]
    [InlineData("GET", "/api/User/GetAllUsers")]
    [InlineData("POST", "/api/Auth/Login")]
    public async Task LegacyRoute_IsNotServed(string method, string route)
    {
        var client = factory.CreateClient();
        client.AuthenticateWithBearer(EducationRoles.Student);

        using var request = new HttpRequestMessage(new HttpMethod(method), route);
        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
