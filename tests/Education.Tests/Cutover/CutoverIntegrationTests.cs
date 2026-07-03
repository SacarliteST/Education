using System.Net;
using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.Courses;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Education.Web.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Tests.Cutover;

public sealed class CutoverIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public CutoverIntegrationTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task LegacyCookieLoginRoute_IsNotServedByNewHost()
    {
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/Auth/Login", new { Login = "Admin", Password = "123" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData(EducationRoles.Student, "/api/v1/student/ping")]
    [InlineData(EducationRoles.Teacher, "/api/v1/teacher/ping")]
    [InlineData(EducationRoles.Admin, "/api/v1/admin/ping")]
    public async Task BearerAuth_WorksForRolePolicies(string role, string url)
    {
        var client = factory.CreateClient();
        client.AuthenticateWithBearer(role);

        var response = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task KeyStudentScenario_RunsOnLegacySchemaWithBearerAuth()
    {
        var client = factory.CreateClient();
        client.AuthenticateWithBearer(EducationRoles.Student);

        var courses = await client.GetFromJsonAsync<IReadOnlyList<CourseResponse>>('/' + ApiRoutes.Courses.StudentCourses);
        var response = await client.PutAsync('/' + ApiRoutes.TestResults.ForStart(factory.Seed.AssignedStartPracticalId), null);

        Assert.Contains(courses!, course => course.Id == factory.Seed.OwnCourseId);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task KeyTeacherScenario_RunsOnLegacySchemaWithBearerAuth()
    {
        var client = factory.CreateClient();
        client.AuthenticateWithBearer(EducationRoles.Teacher);

        var courses = await client.GetFromJsonAsync<IReadOnlyList<CourseResponse>>('/' + ApiRoutes.Courses.TeacherCourses);
        var response = await client.GetAsync('/' + ApiRoutes.TestResults.ForTeacherPracticalProtocols(factory.Seed.ProtocolPracticalId));

        Assert.Contains(courses!, course => course.Id == factory.Seed.OwnCourseId);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task OpenApi_DoesNotExposeLegacyControllerRoutes()
    {
        var client = factory.CreateClient();

        var openApiJson = await client.GetStringAsync("/openapi/v1.json");

        Assert.DoesNotContain("/api/Auth/Login", openApiJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("/api/Student/GetCourses", openApiJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("/api/Teacher/GetCourses", openApiJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("/api/User/GetAllUsers", openApiJson, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("/api/v1/auth/me", openApiJson, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Cors_AllowsConfiguredFrontendOrigin()
    {
        var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/v1/auth/me");
        request.Headers.Add("Origin", "http://localhost:5173");
        request.Headers.Add("Access-Control-Request-Method", "GET");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins));
        Assert.Contains("http://localhost:5173", origins);
    }

    [Fact]
    public async Task DeprecatedLegacyDatabaseFields_ArePhysicallyPreserved()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var userEntity = dbContext.Model.FindEntityType(typeof(Education.Domain.Users.User))!;

        Assert.NotNull(userEntity.FindProperty(nameof(Education.Domain.Users.User.Password)));
        Assert.NotNull(userEntity.FindProperty(nameof(Education.Domain.Users.User.RoleId)));
        Assert.True(await dbContext.Roles.AnyAsync());
        Assert.True(await dbContext.Users.AnyAsync());
    }
}
