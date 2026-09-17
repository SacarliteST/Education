using System.Net;
using System.Net.Http.Json;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Tests;

public class WebSmokeTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public WebSmokeTests(TestWebApplicationFactory factory)
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
    public async Task OpenApi_IsAvailableInDevelopment()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/openapi/v1.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutUser_ReturnsUnauthorized()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task TestAuthHandler_PopulatesSubAndRoleClaims()
    {
        using var client = factory.CreateClient();
        client.AuthenticateWithBearer(TestAuthFixtures.StudentRoles);

        var response = await client.GetFromJsonAsync<AuthMeSmokeResponse>("/api/v1/auth/me");

        Assert.NotNull(response);
        Assert.Equal(TestAuthHandler.TestUserId, response.UserId);
        Assert.Contains("Student", response.Roles);
        Assert.True(response.IsAuthenticated);
    }

    [Fact]
    public async Task TestDatabase_IsCreatedAndSeededPredictably()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();

        Assert.Equal(3, await dbContext.Users.CountAsync());
        Assert.Equal(2, await dbContext.Courses.CountAsync());
        Assert.Equal(6, await dbContext.PracticalMaterials.CountAsync());

        // Сид создаёт связку и для test.user, и для other.student (нужна другим
        // тестам разграничения доступа) — проверяем конкретно тестового пользователя,
        // а не полагаемся на то, что в таблице ровно одна строка.
        Assert.Equal(2, await dbContext.IdentityUserLinks.CountAsync());
        var linkedUser = await dbContext.IdentityUserLinks.SingleAsync(
            link => link.IdentityUserId == TestAuthHandler.TestUserId);
        Assert.Equal(factory.Seed.TestUserId, linkedUser.LegacyUserId);
        Assert.Equal(TestAuthHandler.TestUserId, linkedUser.IdentityUserId);
        Assert.NotEqual(Guid.Empty, factory.Seed.OwnCourseId);
        Assert.NotEqual(Guid.Empty, factory.Seed.OtherCourseId);
    }

    private sealed record AuthMeSmokeResponse(
        Guid UserId,
        IReadOnlyList<string> Roles,
        bool IsAuthenticated);
}
