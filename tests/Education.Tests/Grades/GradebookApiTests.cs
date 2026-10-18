using System.Net;
using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.Grades;
using Education.Domain.Tests;
using Education.Infrastructure.Persistence;
using Education.Tests.Auth;
using Education.Web.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Education.Tests.Grades;

public sealed class GradebookApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public GradebookApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    private async Task CompleteTestAsync(Guid practicalId, int tryNumber)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EducationDbContext>();
        var result = new TestResult(factory.Seed.TestUserId, practicalId, tryNumber);
        result.Complete(1, 1, DateTime.UtcNow);
        db.TestResults.Add(result);
        await db.SaveChangesAsync();
    }

    [Fact]
    public async Task TeacherSeesStudentGrade_SameAsStudentSeesOwn()
    {
        await CompleteTestAsync(factory.Seed.SubmitPracticalId, 95);

        var teacher = factory.CreateClient();
        teacher.AuthenticateAs(EducationRoles.Teacher);
        var rows = await teacher.GetFromJsonAsync<PracticalStudentGradeResponse[]>(
            '/' + ApiRoutes.Grades.ForPracticalStudentGrades(factory.Seed.SubmitPracticalId));

        var row = Assert.Single(rows!, item => item.StudentId == factory.Seed.TestUserId);
        Assert.Equal("passed", row.Status);
        Assert.NotNull(row.Grade);

        var student = factory.CreateClient();
        student.AuthenticateAs(EducationRoles.Student);
        var own = await student.GetFromJsonAsync<PracticalGradeResponse>(
            '/' + ApiRoutes.Grades.ForPracticalGrade(factory.Seed.SubmitPracticalId));
        Assert.Equal(own!.Grade, row.Grade);
    }

    [Fact]
    public async Task Gradebook_ContainsStudentsPracticalsAndSummary()
    {
        await CompleteTestAsync(factory.Seed.SubmitPracticalId, 96);

        var teacher = factory.CreateClient();
        teacher.AuthenticateAs(EducationRoles.Teacher);
        var book = await teacher.GetFromJsonAsync<CourseGradebookResponse>(
            '/' + ApiRoutes.Grades.ForCourseGradebook(factory.Seed.OwnCourseId));

        Assert.Contains(book!.Practicals, item => item.Id == factory.Seed.SubmitPracticalId);
        var student = Assert.Single(book.Students, item => item.StudentId == factory.Seed.TestUserId);
        Assert.Equal(book.Practicals.Count, student.Cells.Count);
        var cell = Assert.Single(student.Cells, item => item.PracticalId == factory.Seed.SubmitPracticalId);
        Assert.True(cell.Assigned);
        Assert.Equal("passed", cell.Status);
        Assert.True(student.PassedCount >= 1);
        Assert.True(student.AssignedCount >= student.PassedCount);
        Assert.NotNull(student.AverageGrade);
    }

    [Fact]
    public async Task Gradebook_UnassignedPractical_IsNotAssignedCell()
    {
        var teacher = factory.CreateClient();
        teacher.AuthenticateAs(EducationRoles.Teacher);
        var book = await teacher.GetFromJsonAsync<CourseGradebookResponse>(
            '/' + ApiRoutes.Grades.ForCourseGradebook(factory.Seed.OwnCourseId));

        var student = Assert.Single(book!.Students, item => item.StudentId == factory.Seed.TestUserId);
        var cell = Assert.Single(student.Cells, item => item.PracticalId == factory.Seed.UnassignedPracticalId);
        Assert.False(cell.Assigned);
        Assert.Null(cell.Grade);
    }

    [Fact]
    public async Task Teacher_CannotReadOtherTeacherGradesOrGradebook()
    {
        var teacher = factory.CreateClient();
        teacher.AuthenticateAs(EducationRoles.Teacher);

        var practical = await teacher.GetAsync('/' + ApiRoutes.Grades.ForPracticalStudentGrades(factory.Seed.OtherTeacherPracticalId));
        var course = await teacher.GetAsync('/' + ApiRoutes.Grades.ForCourseGradebook(factory.Seed.OtherCourseId));

        Assert.Equal(HttpStatusCode.Forbidden, practical.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, course.StatusCode);
    }

    [Fact]
    public async Task Student_CannotReadTeacherGradeEndpoints()
    {
        var student = factory.CreateClient();
        student.AuthenticateAs(EducationRoles.Student);

        var practical = await student.GetAsync('/' + ApiRoutes.Grades.ForPracticalStudentGrades(factory.Seed.SubmitPracticalId));
        var course = await student.GetAsync('/' + ApiRoutes.Grades.ForCourseGradebook(factory.Seed.OwnCourseId));

        Assert.Equal(HttpStatusCode.Forbidden, practical.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, course.StatusCode);
    }
}
