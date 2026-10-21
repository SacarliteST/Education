using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Education.Tests.Users;

/// <summary>
/// Миграция <c>UnifyUserIdWithIdentity</c> на реальной БД: схема доводится до предыдущей миграции, вносятся данные в старой
/// модели (две пары «учебный id — id Identity» и профиль без связи), затем применяется миграция.
/// </summary>
public sealed class UserIdMigrationTests : IAsyncLifetime
{
    private const string PreviousMigration = "20261018210655_RemoveAdminEvents";
    private const string StudentRole = "10000000-0000-0000-0000-000000000003";

    private readonly PostgreSqlContainer container = new PostgreSqlBuilder("postgres:15-alpine")
        .WithCleanUp(true)
        .Build();

    public Task InitializeAsync() => container.StartAsync();

    public async Task DisposeAsync() => await container.DisposeAsync();

    [Fact]
    public async Task Migration_RewritesUserIdsEverywhere_KeepsInactiveAndOrphans_DropsLinks()
    {
        var options = new DbContextOptionsBuilder<EducationDbContext>()
            .UseNpgsql(container.GetConnectionString())
            .Options;
        await using var db = new EducationDbContext(options);
        var migrator = db.GetService<IMigrator>();
        await migrator.MigrateAsync(PreviousMigration);

        var activeOld = Guid.NewGuid();
        var activeNew = Guid.NewGuid();
        var inactiveOld = Guid.NewGuid();
        var inactiveNew = Guid.NewGuid();
        var orphan = Guid.NewGuid();
        var course = Guid.NewGuid();
        await SeedOldSchemaAsync(db, activeOld, activeNew, inactiveOld, inactiveNew, orphan, course);

        await migrator.MigrateAsync();

        await using var connection = new NpgsqlConnection(container.GetConnectionString());
        await connection.OpenAsync();

        // Идентификаторы и активность.
        Assert.Equal(
            (true, true),
            await ReadUserAsync(connection, activeNew));
        Assert.Equal((false, true), await ReadUserAsync(connection, inactiveNew));
        Assert.Equal((false, true), await ReadUserAsync(connection, orphan));
        Assert.False((await ReadUserAsync(connection, activeOld)).Exists);
        Assert.False((await ReadUserAsync(connection, inactiveOld)).Exists);

        // Ссылки на пользователя переписаны (иначе возврат внешних ключей миграция бы не пережила).
        Assert.Equal(activeNew, await ScalarAsync<Guid>(connection, $"select user_id from \"Courses\" where id = '{course}'"));
        Assert.Equal(activeNew, await ScalarAsync<Guid>(connection, $"select user_id from \"CourseBindUsers\" where course_id = '{course}' and user_id = '{activeNew}'"));
        Assert.Equal(3L, await ScalarAsync<long>(connection, "select count(*) from \"Users\""));

        // Таблицы связей больше нет.
        Assert.Null(await ScalarAsync<string?>(connection, "select to_regclass('\"IdentityUserLinks\"')::text"));
    }

    private static async Task SeedOldSchemaAsync(
        EducationDbContext db, Guid activeOld, Guid activeNew, Guid inactiveOld, Guid inactiveNew, Guid orphan, Guid course)
    {
        string User(Guid id, string login) =>
            $"insert into \"Users\" (id, login, password, first_name, last_name, middle_name, role_id) values ('{id}', '{login}', '', 'F', 'L', 'M', '{StudentRole}');";
        string Link(Guid legacy, Guid identity, bool active) =>
            $"insert into \"IdentityUserLinks\" (id, legacy_user_id, identity_user_id, created_at, is_active) values ('{Guid.NewGuid()}', '{legacy}', '{identity}', now(), {(active ? "true" : "false")});";

        await db.Database.ExecuteSqlRawAsync(
            User(activeOld, "active") + User(inactiveOld, "inactive") + User(orphan, "orphan") +
            Link(activeOld, activeNew, true) + Link(inactiveOld, inactiveNew, false) +
            $"insert into \"Courses\" (id, c_name, description, date, user_id) values ('{course}', 'c', 'd', now(), '{activeOld}');" +
            $"insert into \"CourseBindUsers\" (id, course_id, user_id) values ('{Guid.NewGuid()}', '{course}', '{activeOld}');");
    }

    private static async Task<(bool IsActive, bool Exists)> ReadUserAsync(NpgsqlConnection connection, Guid id)
    {
        await using var command = new NpgsqlCommand($"select is_active from \"Users\" where id = '{id}'", connection);
        var value = await command.ExecuteScalarAsync();
        return value is bool active ? (active, true) : (false, false);
    }

    private static async Task<T> ScalarAsync<T>(NpgsqlConnection connection, string sql)
    {
        await using var command = new NpgsqlCommand(sql, connection);
        var value = await command.ExecuteScalarAsync();
        return value is null or DBNull ? default! : (T)value;
    }
}
