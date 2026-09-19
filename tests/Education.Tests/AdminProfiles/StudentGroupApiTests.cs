using System.Net;
using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.AdminProfiles;
using Education.Tests.Auth;
using Education.Web.Identity;

namespace Education.Tests.AdminProfiles;

/// <summary>
/// Учебная группа студента: поле профиля, фильтр и поиск при назначении, список групп.
/// </summary>
public sealed class StudentGroupApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public StudentGroupApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Admin_CreatesProfile_WithGroup_TrimmedAndReturned()
    {
        var group = UniqueGroup();

        var profile = await CreateProfileAsync("a", $"  {group}  ");

        Assert.Equal(group, profile.Group);
    }

    [Fact]
    public async Task Admin_CreatesProfile_WithoutGroup_GroupIsNull()
    {
        var profile = await CreateProfileAsync("b", null);
        var blank = await CreateProfileAsync("c", "   ");

        Assert.Null(profile.Group);
        Assert.Null(blank.Group);
    }

    [Fact]
    public async Task Admin_UpdatesProfile_ChangesAndClearsGroup()
    {
        var group = UniqueGroup();
        var profile = await CreateProfileAsync("d", group);
        var admin = AdminClient();
        var route = '/' + ApiRoutes.AdminProfiles.ForProfile(profile.LegacyUserId);

        var moved = await admin.PutAsJsonAsync(route, new UpdateAdminProfileRequest(
            profile.Login, profile.FirstName, profile.LastName, profile.MiddleName, group + "-2"));
        moved.EnsureSuccessStatusCode();
        var afterMove = (await moved.Content.ReadFromJsonAsync<AdminProfileResponse>())!;

        var cleared = await admin.PutAsJsonAsync(route, new UpdateAdminProfileRequest(
            profile.Login, profile.FirstName, profile.LastName, profile.MiddleName));
        cleared.EnsureSuccessStatusCode();
        var afterClear = (await cleared.Content.ReadFromJsonAsync<AdminProfileResponse>())!;

        Assert.Equal(group + "-2", afterMove.Group);
        Assert.Null(afterClear.Group);
    }

    [Fact]
    public async Task Admin_ProfilesList_ContainsGroup()
    {
        var group = UniqueGroup();
        var profile = await CreateProfileAsync("e", group);

        var list = await AdminClient().GetFromJsonAsync<List<AdminProfileResponse>>(
            '/' + ApiRoutes.AdminProfiles.ProfilesList);

        Assert.Equal(group, list!.Single(item => item.LegacyUserId == profile.LegacyUserId).Group);
    }

    [Fact]
    public async Task Admin_GroupLongerThanLimit_Returns400()
    {
        var response = await AdminClient().PostAsJsonAsync(
            '/' + ApiRoutes.AdminProfiles.ProfilesList,
            new CreateAdminProfileRequest(
                Guid.NewGuid(), "long." + Guid.NewGuid().ToString("N")[..8], "Имя", "Фамилия", "Отч",
                ProfileRole.Student, new string('Я', 51)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Assignments_ShowGroup_FilterByIt_CaseInsensitive_AndSearchMatchesGroup()
    {
        var group = UniqueGroup();
        var inGroup = await CreateProfileAsync("f", group);
        var inGroupToo = await CreateProfileAsync("g", group);
        var elsewhere = await CreateProfileAsync("h", UniqueGroup());
        var client = TeacherClient();
        var route = '/' + ApiRoutes.AdminProfiles.ForCourseStudentAssignments(factory.Seed.OwnCourseId);

        var byGroup = await GetPageAsync(client, route, $"group={group.ToLowerInvariant()}&pageSize=200");
        var bySearch = await GetPageAsync(client, route, $"search={group}&pageSize=200");
        var percent = await GetPageAsync(client, route, "group=%25");

        Assert.Equal(
            new[] { inGroup.LegacyUserId, inGroupToo.LegacyUserId }.Order().ToArray(),
            byGroup.Items.Select(item => item.LegacyUserId).Order().ToArray());
        Assert.All(byGroup.Items, item => Assert.Equal(group, item.Group));
        Assert.Equal(byGroup.TotalCount, bySearch.TotalCount);
        Assert.DoesNotContain(byGroup.Items, item => item.LegacyUserId == elsewhere.LegacyUserId);
        Assert.Equal(0, percent.TotalCount);
    }

    [Fact]
    public async Task Assignments_GroupFilter_CombinesWithAssignedFilter()
    {
        var group = UniqueGroup();
        var first = await CreateProfileAsync("i", group);
        await CreateProfileAsync("j", group);
        var client = TeacherClient();
        var courseId = factory.Seed.OwnCourseId;
        var route = '/' + ApiRoutes.AdminProfiles.ForCourseStudentAssignments(courseId);

        (await client.PostAsJsonAsync(
            '/' + ApiRoutes.Courses.ForCourseStudentChanges(courseId),
            new ChangeStudentsRequest([first.LegacyUserId], []))).EnsureSuccessStatusCode();

        var assigned = await GetPageAsync(client, route, $"group={group}&assigned=true");
        var notAssigned = await GetPageAsync(client, route, $"group={group}&assigned=false");

        Assert.Equal(first.LegacyUserId, assigned.Items.Single().LegacyUserId);
        Assert.Equal(1, notAssigned.TotalCount);
    }

    [Fact]
    public async Task Assignments_TooLongGroupFilter_Returns400()
    {
        var response = await TeacherClient().GetAsync(
            '/' + ApiRoutes.AdminProfiles.ForCourseStudentAssignments(factory.Seed.OwnCourseId)
            + "?group=" + new string('я', 51));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task StudentGroups_ListsGroupsWithCounts_ForTeacherAndAdmin_NotForStudent()
    {
        var group = UniqueGroup();
        await CreateProfileAsync("k", group);
        await CreateProfileAsync("l", group);

        var teacherView = await TeacherClient().GetFromJsonAsync<List<StudentGroupResponse>>(
            '/' + ApiRoutes.AdminProfiles.StudentGroups);
        var adminView = await AdminClient().GetFromJsonAsync<List<StudentGroupResponse>>(
            '/' + ApiRoutes.AdminProfiles.StudentGroups);
        var student = factory.CreateClient();
        student.AuthenticateAs(EducationRoles.Student);
        var forbidden = await student.GetAsync('/' + ApiRoutes.AdminProfiles.StudentGroups);

        Assert.Equal(2, teacherView!.Single(item => item.Name == group).StudentsCount);
        Assert.Equal(2, adminView!.Single(item => item.Name == group).StudentsCount);
        Assert.Equal(HttpStatusCode.Forbidden, forbidden.StatusCode);
    }

    [Fact]
    public async Task StudentGroups_DoNotCountTeachersOrUngroupedStudents()
    {
        var group = UniqueGroup();
        var admin = AdminClient();
        var response = await admin.PostAsJsonAsync(
            '/' + ApiRoutes.AdminProfiles.ProfilesList,
            new CreateAdminProfileRequest(
                Guid.NewGuid(), "t." + Guid.NewGuid().ToString("N")[..8], "Имя", "Препод", "Отч",
                ProfileRole.Teacher, group));
        response.EnsureSuccessStatusCode();

        var groups = await TeacherClient().GetFromJsonAsync<List<StudentGroupResponse>>(
            '/' + ApiRoutes.AdminProfiles.StudentGroups);

        Assert.DoesNotContain(groups!, item => item.Name == group);
    }

    // ---- helpers ----

    private static string UniqueGroup() => "Г" + Guid.NewGuid().ToString("N")[..8];

    private HttpClient AdminClient()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Admin);
        return client;
    }

    private HttpClient TeacherClient()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);
        return client;
    }

    private async Task<AdminProfileResponse> CreateProfileAsync(string suffix, string? group)
    {
        var response = await AdminClient().PostAsJsonAsync(
            '/' + ApiRoutes.AdminProfiles.ProfilesList,
            new CreateAdminProfileRequest(
                Guid.NewGuid(), $"grp.{suffix}.{Guid.NewGuid():N}"[..24], "Имя", "Фамилия" + suffix, "Отч",
                ProfileRole.Student, group));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AdminProfileResponse>())!;
    }

    private static async Task<StudentAssignmentPageResponse> GetPageAsync(HttpClient client, string route, string query)
    {
        var response = await client.GetAsync(route + "?" + query);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<StudentAssignmentPageResponse>())!;
    }
}
