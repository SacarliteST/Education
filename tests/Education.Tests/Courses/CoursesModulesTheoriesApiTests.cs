using System.Net;
using System.Net.Http.Json;
using Education.Contracts;
using Education.Contracts.Courses;
using Education.Tests.Auth;
using Education.Web.Identity;

namespace Education.Tests.Courses;

public sealed class CoursesModulesTheoriesApiTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly TestWebApplicationFactory factory;

    public CoursesModulesTheoriesApiTests(TestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task TeacherSeesOwnCourses()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);

        var courses = await client.GetFromJsonAsync<IReadOnlyList<CourseResponse>>('/' + ApiRoutes.Courses.TeacherCourses);

        var course = Assert.Single(courses!);
        Assert.Equal(1, course.Id);
    }

    [Fact]
    public async Task TeacherCannotChangeAnotherTeachersCourse()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Teacher);

        var response = await client.DeleteAsync('/' + ApiRoutes.Courses.ForCourse(2));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task StudentSeesOnlyAssignedCourse()
    {
        var client = factory.CreateClient();
        client.AuthenticateAs(EducationRoles.Student);

        var courses = await client.GetFromJsonAsync<IReadOnlyList<CourseResponse>>('/' + ApiRoutes.Courses.StudentCourses);

        var course = Assert.Single(courses!);
        Assert.Equal(1, course.Id);
    }

    [Fact]
    public async Task SharedModulesEndpointRequiresAuthorization()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync('/' + ApiRoutes.Courses.ForCourseModules(1));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SharedTheoryEndpointRequiresAuthorization()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync('/' + ApiRoutes.Theories.ForTheory(1));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
