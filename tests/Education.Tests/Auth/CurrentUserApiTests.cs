using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.Courses;
using Education.Web.Identity;

namespace Education.Tests.Auth;

/// <summary>Контракт «кто я» и базовые сценарии с Bearer-токеном, на которые опирается фронтенд.</summary>
public sealed class CurrentUserApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public CurrentUserApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task AuthMe_ReturnsIdentityOfBearerToken()
    {
        var client = factory.CreateClient();
        client.AuthenticateWithBearer(EducationRoles.Student);

        var response = await client.GetFromJsonAsync<AuthMeResponse>("/api/v1/auth/me");

        Assert.NotNull(response);
        Assert.True(response.IsAuthenticated);
        Assert.Equal(TestAuthHandler.TestUserId, response.UserId);
        Assert.Contains(EducationRoles.Student, response.Roles);
    }

    [Fact]
    public async Task Student_SeesOnlyAssignedCourses()
    {
        var client = factory.CreateClient();
        client.AuthenticateWithBearer(EducationRoles.Student);

        var courses = await client.GetFromJsonAsync<IReadOnlyList<CourseResponse>>('/' + ApiRoutes.Courses.StudentCourses);

        var course = Assert.Single(courses!);
        Assert.Equal(factory.Seed.OwnCourseId, course.Id);
    }

    [Fact]
    public async Task Teacher_SeesOwnCourses()
    {
        var client = factory.CreateClient();
        client.AuthenticateWithBearer(EducationRoles.Teacher);

        var courses = await client.GetFromJsonAsync<IReadOnlyList<CourseResponse>>('/' + ApiRoutes.Courses.TeacherCourses);

        Assert.Contains(courses!, course => course.Id == factory.Seed.OwnCourseId);
    }

    [Fact]
    public async Task Student_CanStartAssignedTest()
    {
        var client = factory.CreateClient();
        client.AuthenticateWithBearer(EducationRoles.Student);

        var response = await client.PutAsync('/' + ApiRoutes.TestResults.ForStart(factory.Seed.AssignedStartPracticalId), null);

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Teacher_CanReadProtocolsOfOwnPractical()
    {
        var client = factory.CreateClient();
        client.AuthenticateWithBearer(EducationRoles.Teacher);

        var response = await client.GetAsync('/' + ApiRoutes.TestResults.ForTeacherPracticalProtocols(factory.Seed.ProtocolPracticalId));

        response.EnsureSuccessStatusCode();
    }

    private sealed record AuthMeResponse(
        Guid UserId,
        string? Email,
        string? Name,
        IReadOnlyList<string> Roles,
        bool IsAuthenticated);
}
