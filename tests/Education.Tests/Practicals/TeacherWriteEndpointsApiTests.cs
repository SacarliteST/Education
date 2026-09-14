using System.Net;
using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.AdminProfiles;
using Education.Contracts.Courses;
using Education.Contracts.Practicals;
using Education.Tests.Auth;
using Education.Web.Identity;

namespace Education.Tests.Practicals;

/// <summary>
/// Покрывает write-эндпоинты контура преподавателя и администратора, добавленные для миграции
/// веба платформы: создание/правка/удаление заданий, удаление практики, назначение студентов.
/// </summary>
public sealed class TeacherWriteEndpointsApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public TeacherWriteEndpointsApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    private HttpClient TeacherClient()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);
        return client;
    }

    private HttpClient AdminClient()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Admin);
        return client;
    }

    private async Task<TaskResponse> CreateTaskAsync(HttpClient client, Guid practicalId, string name)
    {
        var response = await client.PostAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForTasks(practicalId),
            new CreateTaskRequest(name));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TaskResponse>())!;
    }

    // G-1 -----------------------------------------------------------------

    [Fact]
    public async Task Teacher_CreatesTask_InOwnPractical()
    {
        var client = TeacherClient();

        var task = await CreateTaskAsync(client, factory.Seed.AssignedStartPracticalId, "G1 new task");

        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal("G1 new task", task.Name);

        var tasks = await client.GetFromJsonAsync<List<TaskResponse>>(
            '/' + ApiRoutes.Practicals.ForTasks(factory.Seed.AssignedStartPracticalId));
        Assert.Contains(tasks!, item => item.Id == task.Id);
    }

    [Fact]
    public async Task Teacher_CannotCreateTask_InOtherTeacherPractical()
    {
        var client = TeacherClient();

        var response = await client.PostAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForTasks(factory.Seed.OtherTeacherPracticalId),
            new CreateTaskRequest("G1 forbidden task"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateTask_WithEmptyName_ReturnsValidationProblem()
    {
        var client = TeacherClient();

        var response = await client.PostAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForTasks(factory.Seed.AssignedStartPracticalId),
            new CreateTaskRequest(String.Empty));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // G-3 -----------------------------------------------------------------

    [Fact]
    public async Task Teacher_UpdatesOwnTaskText()
    {
        var client = TeacherClient();
        var task = await CreateTaskAsync(client, factory.Seed.AssignedStartPracticalId, "G3 task");

        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Tasks.ForTaskText(task.Id),
            new UpdateTaskTextRequest("Обновлённый текст задания"));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var tasks = await client.GetFromJsonAsync<List<TaskResponse>>(
            '/' + ApiRoutes.Practicals.ForTasks(factory.Seed.AssignedStartPracticalId));
        Assert.Equal("Обновлённый текст задания", tasks!.Single(item => item.Id == task.Id).Text);
    }

    [Fact]
    public async Task Teacher_CannotUpdateOtherTeacherTaskText()
    {
        var client = TeacherClient();

        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Tasks.ForTaskText(factory.Seed.OtherTeacherTaskId),
            new UpdateTaskTextRequest("hack"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // G-2 -----------------------------------------------------------------

    [Fact]
    public async Task Teacher_DeletesOwnTask()
    {
        var client = TeacherClient();
        var task = await CreateTaskAsync(client, factory.Seed.AssignedStartPracticalId, "G2 task");

        var response = await client.DeleteAsync('/' + ApiRoutes.Tasks.ForTask(task.Id));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var tasks = await client.GetFromJsonAsync<List<TaskResponse>>(
            '/' + ApiRoutes.Practicals.ForTasks(factory.Seed.AssignedStartPracticalId));
        Assert.DoesNotContain(tasks!, item => item.Id == task.Id);
    }

    [Fact]
    public async Task Teacher_CannotDeleteOtherTeacherTask()
    {
        var client = TeacherClient();

        var response = await client.DeleteAsync('/' + ApiRoutes.Tasks.ForTask(factory.Seed.OtherTeacherTaskId));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // G-4 -----------------------------------------------------------------

    [Fact]
    public async Task Teacher_DeletesOwnPractical()
    {
        var client = TeacherClient();
        var created = await client.PostAsJsonAsync(
            '/' + ApiRoutes.Practicals.PracticalsList,
            new CreatePracticalRequest(factory.Seed.OwnModuleId, "G4 practical"));
        created.EnsureSuccessStatusCode();
        var practical = (await created.Content.ReadFromJsonAsync<PracticalResponse>())!;

        var response = await client.DeleteAsync('/' + ApiRoutes.Practicals.ForPractical(practical.Id));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var practicals = await client.GetFromJsonAsync<List<PracticalResponse>>(
            '/' + ApiRoutes.Modules.ForModulePracticals(factory.Seed.OwnModuleId));
        Assert.DoesNotContain(practicals!, item => item.Id == practical.Id);
    }

    [Fact]
    public async Task Teacher_CannotDeleteOtherTeacherPractical()
    {
        var client = TeacherClient();

        var response = await client.DeleteAsync(
            '/' + ApiRoutes.Practicals.ForPractical(factory.Seed.OtherTeacherPracticalId));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // G-5 -----------------------------------------------------------------

    [Fact]
    public async Task Teacher_ReplacesOwnCourseStudents()
    {
        var client = TeacherClient();
        var courseId = factory.Seed.OwnCourseId;

        var assign = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Courses.ForCourseStudents(courseId),
            new UpdateCourseStudentsRequest([factory.Seed.OtherStudentId]));
        Assert.Equal(HttpStatusCode.NoContent, assign.StatusCode);

        var assigned = await client.GetFromJsonAsync<List<AssignableStudentResponse>>(
            '/' + ApiRoutes.AdminProfiles.ForCourseAssignableStudents(courseId));
        Assert.True(assigned!.Single(item => item.LegacyUserId == factory.Seed.OtherStudentId).IsAssigned);

        var clear = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Courses.ForCourseStudents(courseId),
            new UpdateCourseStudentsRequest([]));
        Assert.Equal(HttpStatusCode.NoContent, clear.StatusCode);

        var afterClear = await client.GetFromJsonAsync<List<AssignableStudentResponse>>(
            '/' + ApiRoutes.AdminProfiles.ForCourseAssignableStudents(courseId));
        Assert.False(afterClear!.Single(item => item.LegacyUserId == factory.Seed.OtherStudentId).IsAssigned);
    }

    [Fact]
    public async Task Teacher_AssignableStudents_ExcludesNonStudentProfiles()
    {
        var client = TeacherClient();

        var courseStudents = await client.GetFromJsonAsync<List<AssignableStudentResponse>>(
            '/' + ApiRoutes.AdminProfiles.ForCourseAssignableStudents(factory.Seed.OwnCourseId));
        var practicalStudents = await client.GetFromJsonAsync<List<AssignableStudentResponse>>(
            '/' + ApiRoutes.AdminProfiles.ForPracticalAssignableStudents(factory.Seed.SubmitPracticalId));

        // связанный студенческий профиль в списке есть
        Assert.Contains(courseStudents!, item => item.LegacyUserId == factory.Seed.OtherStudentId);
        Assert.Contains(practicalStudents!, item => item.LegacyUserId == factory.Seed.OtherStudentId);
        // связанный преподавательский профиль — нет
        Assert.DoesNotContain(courseStudents!, item => item.LegacyUserId == factory.Seed.TestUserId);
        Assert.DoesNotContain(practicalStudents!, item => item.LegacyUserId == factory.Seed.TestUserId);
    }

    [Fact]
    public async Task Teacher_SetCourseStudents_UnknownUserId_Returns400()
    {
        var client = TeacherClient();

        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Courses.ForCourseStudents(factory.Seed.OwnCourseId),
            new UpdateCourseStudentsRequest([Guid.NewGuid()]));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Teacher_SetCourseStudents_ForUnknownCourse_ReturnsForbidden()
    {
        var client = TeacherClient();

        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Courses.ForCourseStudents(Guid.NewGuid()),
            new UpdateCourseStudentsRequest([factory.Seed.TestUserId]));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Student_CannotReplaceCourseStudents()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);

        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Courses.ForCourseStudents(factory.Seed.OwnCourseId),
            new UpdateCourseStudentsRequest([factory.Seed.TestUserId]));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    // G-6 -----------------------------------------------------------------

    [Fact]
    public async Task Teacher_ReplacesOwnPracticalStudents()
    {
        var client = TeacherClient();
        var practicalId = factory.Seed.SubmitPracticalId;

        var assign = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForStudents(practicalId),
            new UpdatePracticalStudentsRequest([factory.Seed.OtherStudentId]));
        Assert.Equal(HttpStatusCode.NoContent, assign.StatusCode);

        var assigned = await client.GetFromJsonAsync<List<AssignableStudentResponse>>(
            '/' + ApiRoutes.AdminProfiles.ForPracticalAssignableStudents(practicalId));
        Assert.True(assigned!.Single(item => item.LegacyUserId == factory.Seed.OtherStudentId).IsAssigned);
    }

    [Fact]
    public async Task Teacher_SetPracticalStudents_ForUnknownPractical_ReturnsForbidden()
    {
        var client = TeacherClient();

        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForStudents(Guid.NewGuid()),
            new UpdatePracticalStudentsRequest([factory.Seed.TestUserId]));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Admin_WithoutTeacherRole_CannotReplacePracticalStudents()
    {
        var client = AdminClient();

        var response = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForStudents(factory.Seed.SubmitPracticalId),
            new UpdatePracticalStudentsRequest([factory.Seed.TestUserId]));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
