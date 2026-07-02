using Education.Domain.Courses;
using Education.Domain.Materials;
using Education.Domain.Practicals;
using Education.Domain.Tests;
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
    private readonly string fileStorageRoot = Path.Combine(Path.GetTempPath(), "education-tests-" + Guid.NewGuid());

    public string FileStorageRoot => fileStorageRoot;

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

        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var testUser = new User("test.user", "Test", "User", String.Empty, 2);
        var otherTeacher = new User("other.teacher", "Other", "Teacher", String.Empty, 2);
        var otherStudent = new User("other.student", "Other", "Student", String.Empty, 3);

        await dbContext.Users.AddRangeAsync(testUser, otherTeacher, otherStudent);
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
        var otherModule = new Module(otherCourse.Id, "Other module");
        await dbContext.Modules.AddRangeAsync(module, otherModule);
        await dbContext.SaveChangesAsync();

        await dbContext.TheoreticalMaterials.AddAsync(new TheoreticalMaterial(module.Id, "Theory", "Theory text"));
        await dbContext.SaveChangesAsync();

        var assignedStartPractical = new PracticalMaterial(module.Id, "Assigned start practical");
        var unassignedPractical = new PracticalMaterial(module.Id, "Unassigned practical");
        var submitPractical = new PracticalMaterial(module.Id, "Submit practical");
        var limitedPractical = new PracticalMaterial(module.Id, "Limited practical");
        var protocolPractical = new PracticalMaterial(module.Id, "Protocol practical");
        var otherTeacherPractical = new PracticalMaterial(otherModule.Id, "Other teacher practical");
        await dbContext.PracticalMaterials.AddRangeAsync(
            assignedStartPractical,
            unassignedPractical,
            submitPractical,
            limitedPractical,
            protocolPractical,
            otherTeacherPractical);
        await dbContext.SaveChangesAsync();

        await dbContext.PracticalBindUsers.AddRangeAsync(
            new PracticalBindUser(assignedStartPractical.Id, testUser.Id),
            new PracticalBindUser(submitPractical.Id, testUser.Id),
            new PracticalBindUser(limitedPractical.Id, testUser.Id),
            new PracticalBindUser(protocolPractical.Id, testUser.Id),
            new PracticalBindUser(assignedStartPractical.Id, otherStudent.Id));
        await dbContext.SaveChangesAsync();

        var assignedTask = new Case(assignedStartPractical.Id, "Assigned task", "Upload solution");
        var otherTeacherTask = new Case(otherTeacherPractical.Id, "Other teacher task", "Other upload");
        await dbContext.Cases.AddRangeAsync(assignedTask, otherTeacherTask);
        await dbContext.SaveChangesAsync();

        Directory.CreateDirectory(fileStorageRoot);
        await File.WriteAllTextAsync(Path.Combine(fileStorageRoot, "other-student.txt"), "other student file");
        await File.WriteAllTextAsync(Path.Combine(fileStorageRoot, "other-teacher.txt"), "other teacher file");

        await dbContext.CaseFiles.AddRangeAsync(
            new CaseFile(assignedTask.Id, otherStudent.Id, "other-student.txt", "other-student.txt"),
            new CaseFile(otherTeacherTask.Id, testUser.Id, "other-teacher.txt", "other-teacher.txt"));

        var startQuestion = CreateSingleChoiceQuestion(module.Id, "Start question");
        var submitQuestion = CreateSingleChoiceQuestion(module.Id, "Submit question");
        var limitedQuestion = CreateSingleChoiceQuestion(module.Id, "Limited question");
        var protocolQuestion = CreateSingleChoiceQuestion(module.Id, "Protocol question");
        await dbContext.Questions.AddRangeAsync(startQuestion, submitQuestion, limitedQuestion, protocolQuestion);
        await dbContext.SaveChangesAsync();

        await dbContext.PracticalMaterialBindQuestions.AddRangeAsync(
            new PracticalMaterialBindQuestion(assignedStartPractical.Id, startQuestion.Id),
            new PracticalMaterialBindQuestion(submitPractical.Id, submitQuestion.Id),
            new PracticalMaterialBindQuestion(limitedPractical.Id, limitedQuestion.Id),
            new PracticalMaterialBindQuestion(protocolPractical.Id, protocolQuestion.Id));
        await dbContext.SaveChangesAsync();

        var completedLimitedResult = new TestResult(testUser.Id, limitedPractical.Id, 1);
        completedLimitedResult.Complete(1, 1, DateTime.UtcNow);
        var completedProtocolResult = new TestResult(testUser.Id, protocolPractical.Id, 1);
        completedProtocolResult.Complete(1, 1, DateTime.UtcNow);
        await dbContext.TestResults.AddRangeAsync(completedLimitedResult, completedProtocolResult);
        await dbContext.SaveChangesAsync();
    }

    private static Question CreateSingleChoiceQuestion(long moduleId, string text)
    {
        const string body = """
            {"answers":[{"id":"a","text":"Right"},{"id":"b","text":"Wrong"}]}
            """;
        const string answer = """
            {"answers":[{"id":"a","text":"Right"},{"id":"b","text":"Wrong"}],"correctAnswerId":"a"}
            """;

        return new Question(moduleId, (long)QuestionKind.SingleChoice, text, body, answer, 1);
    }
}
