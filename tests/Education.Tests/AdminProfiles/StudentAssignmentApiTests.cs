using System.Net;
using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.AdminProfiles;
using Education.Tests.Auth;
using Education.Web.Identity;

namespace Education.Tests.AdminProfiles;

/// <summary>
/// Постраничный список студентов для назначения и точечное изменение набора (добавить / убрать).
/// </summary>
public sealed class StudentAssignmentApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public StudentAssignmentApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Page_SplitsMatchingStudents_SortedByLastName_WithTotals()
    {
        var group = await CreateStudentsAsync(5);
        var client = TeacherClient();
        var route = '/' + ApiRoutes.AdminProfiles.ForCourseStudentAssignments(factory.Seed.OwnCourseId);

        var first = await GetPageAsync(client, route, $"search={group.Marker}&pageSize=2&page=1");
        var last = await GetPageAsync(client, route, $"search={group.Marker}&pageSize=2&page=3");

        Assert.Equal(5, first.TotalCount);
        Assert.Equal(2, first.Items.Count);
        Assert.Equal(1, first.Page);
        Assert.Equal(2, first.PageSize);
        Assert.Equal(first.Items.Select(item => item.FullName).Order().ToArray(), first.Items.Select(item => item.FullName).ToArray());
        Assert.Single(last.Items);
        Assert.Equal(5, last.TotalCount);
    }

    [Fact]
    public async Task Page_Search_MatchesLoginCaseInsensitive_AndTreatsPercentLiterally()
    {
        var group = await CreateStudentsAsync(2);
        var client = TeacherClient();
        var route = '/' + ApiRoutes.AdminProfiles.ForCourseStudentAssignments(factory.Seed.OwnCourseId);

        var byLogin = await GetPageAsync(client, route, $"search={group.Marker.ToUpperInvariant()}");
        var percent = await GetPageAsync(client, route, "search=%25");

        Assert.Equal(2, byLogin.TotalCount);
        Assert.All(byLogin.Items, item => Assert.Contains(group.Marker, item.Login));
        Assert.Equal(0, percent.TotalCount);
    }

    [Fact]
    public async Task Page_AssignedFilter_AndAssignedCount_ReflectChanges()
    {
        var group = await CreateStudentsAsync(3);
        var client = TeacherClient();
        var courseId = factory.Seed.OwnCourseId;
        var route = '/' + ApiRoutes.AdminProfiles.ForCourseStudentAssignments(courseId);
        var before = await GetPageAsync(client, route, $"search={group.Marker}");

        var change = await client.PostAsJsonAsync(
            '/' + ApiRoutes.Courses.ForCourseStudentChanges(courseId),
            new ChangeStudentsRequest([group.Ids[0], group.Ids[1]], []));
        Assert.Equal(HttpStatusCode.NoContent, change.StatusCode);

        var assigned = await GetPageAsync(client, route, $"search={group.Marker}&assigned=true");
        var unassigned = await GetPageAsync(client, route, $"search={group.Marker}&assigned=false");
        var after = await GetPageAsync(client, route, $"search={group.Marker}");

        Assert.Equal(2, assigned.TotalCount);
        Assert.All(assigned.Items, item => Assert.True(item.IsAssigned));
        Assert.Equal(1, unassigned.TotalCount);
        Assert.False(unassigned.Items.Single().IsAssigned);
        // счётчик назначенных не зависит от поиска и фильтра
        Assert.Equal(before.AssignedCount + 2, after.AssignedCount);
    }

    [Fact]
    public async Task Change_AddsAndRemoves_WithoutTouchingOtherAssignments()
    {
        var group = await CreateStudentsAsync(2);
        var client = TeacherClient();
        var courseId = factory.Seed.OwnCourseId;
        var route = '/' + ApiRoutes.AdminProfiles.ForCourseStudentAssignments(courseId);
        var changeRoute = '/' + ApiRoutes.Courses.ForCourseStudentChanges(courseId);

        var replace = await client.PutAsJsonAsync(
            '/' + ApiRoutes.Courses.ForCourseStudents(courseId),
            new Education.Contracts.Courses.UpdateCourseStudentsRequest([factory.Seed.OtherStudentId]));
        Assert.Equal(HttpStatusCode.NoContent, replace.StatusCode);

        var add = await client.PostAsJsonAsync(changeRoute, new ChangeStudentsRequest([group.Ids[0]], []));
        Assert.Equal(HttpStatusCode.NoContent, add.StatusCode);
        var remove = await client.PostAsJsonAsync(changeRoute, new ChangeStudentsRequest([group.Ids[1]], [group.Ids[0]]));
        Assert.Equal(HttpStatusCode.NoContent, remove.StatusCode);

        var assigned = await GetPageAsync(client, route, "assigned=true&pageSize=200");
        Assert.Contains(assigned.Items, item => item.LegacyUserId == factory.Seed.OtherStudentId);
        Assert.Contains(assigned.Items, item => item.LegacyUserId == group.Ids[1]);
        Assert.DoesNotContain(assigned.Items, item => item.LegacyUserId == group.Ids[0]);
    }

    [Fact]
    public async Task Change_IsIdempotent_RepeatedAddDoesNotDuplicate()
    {
        var group = await CreateStudentsAsync(1);
        var client = TeacherClient();
        var courseId = factory.Seed.OwnCourseId;
        var route = '/' + ApiRoutes.AdminProfiles.ForCourseStudentAssignments(courseId);
        var changeRoute = '/' + ApiRoutes.Courses.ForCourseStudentChanges(courseId);
        var request = new ChangeStudentsRequest([group.Ids[0]], []);

        (await client.PostAsJsonAsync(changeRoute, request)).EnsureSuccessStatusCode();
        var once = await GetPageAsync(client, route, "assigned=true");
        (await client.PostAsJsonAsync(changeRoute, request)).EnsureSuccessStatusCode();
        var twice = await GetPageAsync(client, route, "assigned=true");

        Assert.Equal(once.AssignedCount, twice.AssignedCount);
        // снятие того, кого нет в наборе, тоже не ошибка
        var removeMissing = await client.PostAsJsonAsync(changeRoute, new ChangeStudentsRequest([], [Guid.NewGuid()]));
        Assert.Equal(HttpStatusCode.NoContent, removeMissing.StatusCode);
    }

    [Fact]
    public async Task Practical_PageAndChange_Work()
    {
        var group = await CreateStudentsAsync(2);
        var client = TeacherClient();
        var practicalId = factory.Seed.SubmitPracticalId;

        var change = await client.PostAsJsonAsync(
            '/' + ApiRoutes.Practicals.ForStudentChanges(practicalId),
            new ChangeStudentsRequest([group.Ids[0]], []));
        Assert.Equal(HttpStatusCode.NoContent, change.StatusCode);

        var page = await GetPageAsync(
            client,
            '/' + ApiRoutes.AdminProfiles.ForPracticalStudentAssignments(practicalId),
            $"search={group.Marker}&assigned=true");
        Assert.Equal(group.Ids[0], page.Items.Single().LegacyUserId);
    }

    [Fact]
    public async Task Change_UnknownStudentInAdd_Returns400()
    {
        var response = await TeacherClient().PostAsJsonAsync(
            '/' + ApiRoutes.Courses.ForCourseStudentChanges(factory.Seed.OwnCourseId),
            new ChangeStudentsRequest([Guid.NewGuid()], []));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Change_SameStudentInAddAndRemove_Returns400()
    {
        var id = Guid.NewGuid();

        var response = await TeacherClient().PostAsJsonAsync(
            '/' + ApiRoutes.Courses.ForCourseStudentChanges(factory.Seed.OwnCourseId),
            new ChangeStudentsRequest([id], [id]));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Change_TooManyIds_Returns400()
    {
        var ids = Enumerable.Range(0, 2001).Select(_ => Guid.NewGuid()).ToArray();

        var response = await TeacherClient().PostAsJsonAsync(
            '/' + ApiRoutes.Courses.ForCourseStudentChanges(factory.Seed.OwnCourseId),
            new ChangeStudentsRequest([], ids));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("pageSize=201")]
    [InlineData("pageSize=0")]
    [InlineData("page=0")]
    public async Task Page_InvalidPaging_Returns400(string query)
    {
        var response = await TeacherClient().GetAsync(
            '/' + ApiRoutes.AdminProfiles.ForCourseStudentAssignments(factory.Seed.OwnCourseId) + "?" + query);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Page_And_Change_ForUnknownCourse_ReturnForbidden()
    {
        var client = TeacherClient();
        var unknown = Guid.NewGuid();

        var page = await client.GetAsync('/' + ApiRoutes.AdminProfiles.ForCourseStudentAssignments(unknown));
        var change = await client.PostAsJsonAsync(
            '/' + ApiRoutes.Courses.ForCourseStudentChanges(unknown),
            new ChangeStudentsRequest([], []));

        Assert.Equal(HttpStatusCode.Forbidden, page.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, change.StatusCode);
    }

    [Fact]
    public async Task Student_CannotUsePageOrChange()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);
        var courseId = factory.Seed.OwnCourseId;

        var page = await client.GetAsync('/' + ApiRoutes.AdminProfiles.ForCourseStudentAssignments(courseId));
        var change = await client.PostAsJsonAsync(
            '/' + ApiRoutes.Courses.ForCourseStudentChanges(courseId),
            new ChangeStudentsRequest([], []));

        Assert.Equal(HttpStatusCode.Forbidden, page.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, change.StatusCode);
    }

    // ---- helpers ----

    private HttpClient TeacherClient()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);
        return client;
    }

    private static async Task<StudentAssignmentPageResponse> GetPageAsync(HttpClient client, string route, string query)
    {
        var response = await client.GetAsync(route + "?" + query);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<StudentAssignmentPageResponse>())!;
    }

    /// <summary>Заводит связанных студентов с общей уникальной меткой в логине и фамилии.</summary>
    private async Task<(string Marker, IReadOnlyList<Guid> Ids)> CreateStudentsAsync(int count)
    {
        var admin = factory.CreateClient();
        admin.AuthenticateAs(EducationRoles.Admin);
        var marker = "sa" + Guid.NewGuid().ToString("N")[..8];
        var ids = new List<Guid>();

        for (var index = 0; index < count; index++)
        {
            var response = await admin.PostAsJsonAsync(
                '/' + ApiRoutes.AdminProfiles.ProfilesList,
                new CreateAdminProfileRequest(
                    Guid.NewGuid(), $"{marker}.{index}", "Имя", $"Ф{marker}{(char)('а' + index)}", "Отч"));
            response.EnsureSuccessStatusCode();
            ids.Add((await response.Content.ReadFromJsonAsync<AdminProfileResponse>())!.LegacyUserId);
        }

        return (marker, ids);
    }
}
