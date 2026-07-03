using System.Net;
using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.Courses;
using Education.Tests.Auth;
using Education.Web.Identity;

namespace Education.Tests.FrontendContracts;

public sealed class FrontendContractTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public FrontendContractTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task AuthMe_WithBearerToken_ReturnsCurrentUser()
    {
        var client = factory.CreateClient();
        client.AuthenticateWithBearer(EducationRoles.Student);

        var response = await client.GetFromJsonAsync<AuthMeContractResponse>("/api/v1/auth/me");

        Assert.NotNull(response);
        Assert.True(response.IsAuthenticated);
        Assert.Equal(TestAuthHandler.TestUserId, response.UserId);
        Assert.Contains(EducationRoles.Student, response.Roles);
    }

    [Fact]
    public async Task ProtectedEducationRequest_WithBearerToken_ReturnsData()
    {
        var client = factory.CreateClient();
        client.AuthenticateWithBearer(EducationRoles.Student);

        var courses = await client.GetFromJsonAsync<IReadOnlyList<CourseResponse>>('/' + ApiRoutes.Courses.StudentCourses);

        var course = Assert.Single(courses!);
        Assert.Equal(factory.Seed.OwnCourseId, course.Id);
    }

    [Fact]
    public async Task ProtectedEducationRequest_WithoutToken_ReturnsUnauthorized()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync('/' + ApiRoutes.Courses.StudentCourses);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AdminRoute_WithStudentBearerToken_ReturnsForbidden()
    {
        var client = factory.CreateClient();
        client.AuthenticateWithBearer(EducationRoles.Student);

        var response = await client.GetAsync('/' + ApiRoutes.AdminProfiles.ProfilesList);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/Auth/IsSignedIn")]
    [InlineData("/api/Student/GetCourses")]
    [InlineData("/api/Teacher/GetCourses")]
    [InlineData("/api/Shared/GetModules?courseId=1")]
    public async Task LegacyFrontendRoutes_AreNotServedByEducationApi(string legacyRoute)
    {
        var client = factory.CreateClient();
        client.AuthenticateWithBearer(EducationRoles.Student);

        var response = await client.GetAsync(legacyRoute);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed record AuthMeContractResponse(
        Guid UserId,
        string? Email,
        string? Name,
        IReadOnlyList<string> Roles,
        bool IsAuthenticated);
}
