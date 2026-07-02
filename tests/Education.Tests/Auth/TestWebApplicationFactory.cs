using Education.Domain.Courses;
using Education.Domain.Materials;
using Education.Domain.Users;
using Education.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Education.Tests.Auth;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string TestDbName = "education_test";
    private const string TestUser = "postgresTestUser";
    private const string TestPassword = "postgresTestPassword";
    private const string FallbackConnectionString =
        "Host=localhost;Database=education_test;Username=postgres;Password=postgres";

    private string? connectionString;

    private readonly PostgreSqlContainer postgreSqlContainer = new PostgreSqlBuilder()
        .WithDatabase(TestDbName)
        .WithUsername(TestUser)
        .WithPassword(TestPassword)
        .WithCleanUp(true)
        .Build();

    public async Task InitializeAsync()
    {
        await postgreSqlContainer.StartAsync();
        connectionString = postgreSqlContainer.GetConnectionString();
        await SeedDatabaseAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration(configurationBuilder =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Default"] = connectionString ?? FallbackConnectionString,
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                    options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                    options.DefaultForbidScheme = TestAuthHandler.AuthenticationScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.AuthenticationScheme,
                    _ => { });
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await postgreSqlContainer.StopAsync();
    }

    private async Task SeedDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var testUser = new User("test.user", "Test", "User", String.Empty, 2);
        var otherTeacher = new User("other.teacher", "Other", "Teacher", String.Empty, 2);

        await dbContext.Users.AddRangeAsync(testUser, otherTeacher);
        await dbContext.SaveChangesAsync();

        await dbContext.IdentityUserLinks.AddAsync(new IdentityUserLink
        {
            LegacyUserId = testUser.Id,
            IdentityUserId = TestAuthHandler.TestUserId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        });

        var ownCourse = new Course(
            "Teacher course",
            "Own course",
            DateTimeOffset.Parse("2025-01-01T00:00:00Z"),
            testUser.Id);
        var otherCourse = new Course(
            "Other course",
            "Other course",
            DateTimeOffset.Parse("2025-02-01T00:00:00Z"),
            otherTeacher.Id);

        await dbContext.Courses.AddRangeAsync(ownCourse, otherCourse);
        await dbContext.SaveChangesAsync();

        await dbContext.CourseBindUsers.AddAsync(new CourseBindUser(ownCourse.Id, testUser.Id));

        var module = new Module(ownCourse.Id, "Own module");
        await dbContext.Modules.AddAsync(module);
        await dbContext.SaveChangesAsync();

        await dbContext.TheoreticalMaterials.AddAsync(new TheoreticalMaterial(module.Id, "Theory", "Theory text"));
        await dbContext.SaveChangesAsync();
    }
}
