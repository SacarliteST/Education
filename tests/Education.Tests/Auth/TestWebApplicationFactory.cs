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
    private readonly string fileStorageRoot = Path.Combine(Path.GetTempPath(), "education-tests-" + Guid.NewGuid());

    public string FileStorageRoot => fileStorageRoot;

    internal TestSeedSnapshot Seed { get; private set; } = TestSeedSnapshot.Empty;

    private readonly PostgreSqlContainer postgreSqlContainer = new PostgreSqlBuilder("postgres:15-alpine")
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
                ["FileStorage:RootPath"] = fileStorageRoot,
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
        if (Directory.Exists(fileStorageRoot))
        {
            Directory.Delete(fileStorageRoot, true);
        }
    }

    private async Task SeedDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();

        Seed = await new TestSeedDataBuilder(fileStorageRoot).SeedAsync(dbContext);
    }
}
