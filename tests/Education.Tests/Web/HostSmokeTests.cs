using System.Net;
using Education.Tests.Auth;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Tests.Web;

/// <summary>Хост поднимается, отдаёт health/OpenAPI, CORS и тестовую БД с ожидаемым сидом.</summary>
public sealed class HostSmokeTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public HostSmokeTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task OpenApi_ExposesCurrentRoutes_AndNoLegacyControllers()
    {
        using var client = factory.CreateClient();

        var openApiJson = await client.GetStringAsync("/openapi/v1.json");

        Assert.Contains("/api/v1/auth/me", openApiJson, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("/api/v1/courses/{courseId}/class-windows", openApiJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("/api/Auth/Login", openApiJson, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("/api/Student/GetCourses", openApiJson, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Cors_AllowsConfiguredFrontendOrigin()
    {
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Options, "/api/v1/auth/me");
        request.Headers.Add("Origin", "http://localhost:5173");
        request.Headers.Add("Access-Control-Request-Method", "GET");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins));
        Assert.Contains("http://localhost:5173", origins);
    }

    [Fact]
    public async Task TestDatabase_IsCreatedAndSeededPredictably()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();

        Assert.Equal(3, await dbContext.Users.CountAsync());
        Assert.Equal(2, await dbContext.Courses.CountAsync());
        Assert.Equal(6, await dbContext.PracticalMaterials.CountAsync());

        // Учебный id пользователя равен id в Identity (claim sub тестового токена).
        var seededUser = await dbContext.Users.SingleAsync(user => user.Id == TestAuthHandler.TestUserId);
        Assert.Equal(factory.Seed.TestUserId, seededUser.Id);
        Assert.True(seededUser.IsActive);
        Assert.NotEqual(Guid.Empty, factory.Seed.OwnCourseId);
        Assert.NotEqual(Guid.Empty, factory.Seed.OtherCourseId);
    }
}
