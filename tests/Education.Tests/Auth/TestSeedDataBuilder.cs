using Education.Domain.Courses;
using Education.Domain.Materials;
using Education.Domain.Practicals;
using Education.Domain.Tests;
using Education.Domain.Users;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Tests.Auth;

internal sealed class TestSeedDataBuilder(string fileStorageRoot)
{
    public async Task<TestSeedSnapshot> SeedAsync(EducationDbContext dbContext)
    {
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();

        var testUser = new User("test.user", "Test", "User", String.Empty, RoleIds.Teacher);
        var otherTeacher = new User("other.teacher", "Other", "Teacher", String.Empty, RoleIds.Teacher);
        var otherStudent = new User("other.student", "Other", "Student", String.Empty, RoleIds.Student);

        await dbContext.Users.AddRangeAsync(testUser, otherTeacher, otherStudent);
        await dbContext.SaveChangesAsync();

        await dbContext.IdentityUserLinks.AddRangeAsync(
            new IdentityUserLink
            {
                LegacyUserId = testUser.Id,
                IdentityUserId = TestAuthHandler.TestUserId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
            new IdentityUserLink
            {
                LegacyUserId = otherStudent.Id,
                IdentityUserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
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

        var otherStudentCaseFile = new CaseFile(assignedTask.Id, otherStudent.Id, "other-student.txt", "other-student.txt");
        var otherTeacherCaseFile = new CaseFile(otherTeacherTask.Id, testUser.Id, "other-teacher.txt", "other-teacher.txt");
        await dbContext.CaseFiles.AddRangeAsync(otherStudentCaseFile, otherTeacherCaseFile);

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

        return new TestSeedSnapshot(
            testUser.Id,
            otherTeacher.Id,
            otherStudent.Id,
            ownCourse.Id,
            otherCourse.Id,
            module.Id,
            otherModule.Id,
            assignedStartPractical.Id,
            unassignedPractical.Id,
            submitPractical.Id,
            limitedPractical.Id,
            protocolPractical.Id,
            otherTeacherPractical.Id,
            assignedTask.Id,
            otherTeacherTask.Id,
            submitQuestion.Id,
            otherStudentCaseFile.Id,
            otherTeacherCaseFile.Id,
            completedLimitedResult.Id,
            completedProtocolResult.Id);
    }

    private static Question CreateSingleChoiceQuestion(Guid moduleId, string text)
    {
        const string body = """
            {"answers":[{"id":"a","text":"Right"},{"id":"b","text":"Wrong"}]}
            """;
        const string answer = """
            {"answers":[{"id":"a","text":"Right"},{"id":"b","text":"Wrong"}],"correctAnswerId":"a"}
            """;

        return new Question(moduleId, QuestionTypeIds.SingleChoice, text, body, answer, 1);
    }
}

internal sealed record TestSeedSnapshot(
    Guid TestUserId,
    Guid OtherTeacherId,
    Guid OtherStudentId,
    Guid OwnCourseId,
    Guid OtherCourseId,
    Guid OwnModuleId,
    Guid OtherModuleId,
    Guid AssignedStartPracticalId,
    Guid UnassignedPracticalId,
    Guid SubmitPracticalId,
    Guid LimitedPracticalId,
    Guid ProtocolPracticalId,
    Guid OtherTeacherPracticalId,
    Guid AssignedTaskId,
    Guid OtherTeacherTaskId,
    Guid SubmitQuestionId,
    Guid OtherStudentCaseFileId,
    Guid OtherTeacherCaseFileId,
    Guid CompletedLimitedResultId,
    Guid CompletedProtocolResultId)
{
    public static TestSeedSnapshot Empty { get; } = new(
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty,
        Guid.Empty);
}
