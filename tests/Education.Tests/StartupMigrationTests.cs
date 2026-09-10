using Education.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Education.Tests;

// TD-011: вне Development миграции при старте применяются только по флагу
// Database:ApplyMigrationsOnStartup. Оба сценария — на реальной пустой PostgreSQL.
public sealed class StartupMigrationTests
{
    [Fact]
    public async Task Production_WithFlag_AppliesMigrationsOnStartup()
    {
        await using var factory = new MigrationStartupFactory(applyMigrationsOnStartup: true);
        await factory.InitializeAsync();

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EducationDbContext>();

        Assert.NotEmpty(await dbContext.Database.GetAppliedMigrationsAsync());
        Assert.Empty(await dbContext.Database.GetPendingMigrationsAsync());
        // Схема действительно накатилась: сид ролей из HasData внутри InitialCreate доступен.
        Assert.True(await dbContext.Roles.AnyAsync());
    }

    [Fact]
    public async Task Production_WithoutFlag_LeavesMigrationsPending()
    {
        await using var factory = new MigrationStartupFactory(applyMigrationsOnStartup: false);
        await factory.InitializeAsync();

        using var scope = factory.Services.CreateScope();
        var database = scope.ServiceProvider.GetRequiredService<EducationDbContext>().Database;

        Assert.Empty(await database.GetAppliedMigrationsAsync());
        Assert.NotEmpty(await database.GetPendingMigrationsAsync());
    }

    private sealed class MigrationStartupFactory(bool applyMigrationsOnStartup)
        : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly string fileStorageRoot =
            Path.Combine(Path.GetTempPath(), "education-migration-tests-" + Guid.NewGuid());

        private readonly PostgreSqlContainer container = new PostgreSqlBuilder("postgres:15-alpine")
            .WithCleanUp(true)
            .Build();

        public async Task InitializeAsync()
        {
            await container.StartAsync();
            // Форсируем построение хоста → выполняется стартовый блок миграций Program.cs.
            _ = Services;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Production");
            builder.ConfigureAppConfiguration(configuration =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Default"] = container.GetConnectionString(),
                    ["FileStorage:RootPath"] = fileStorageRoot,
                    ["Kafka:BootstrapServers"] = "localhost:9092",
                    ["Kafka:ConsumerGroupId"] = "education-migration-test",
                    ["Database:ApplyMigrationsOnStartup"] = applyMigrationsOnStartup ? "true" : "false",
                });
            });
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await container.StopAsync();
            if (Directory.Exists(fileStorageRoot))
            {
                Directory.Delete(fileStorageRoot, true);
            }
        }
    }
}
