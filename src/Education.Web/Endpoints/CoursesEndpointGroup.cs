using Education.Application.Courses;
using Education.Contracts;
using Education.Contracts.Courses;
using Education.Web.Identity;

namespace Education.Web.Endpoints;

public static class CoursesEndpointGroup
{
    public static IEndpointRouteBuilder MapCoursesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Courses.TeacherCourses, async (ICoursesService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetTeacherCoursesAsync(cancellationToken)))
            .WithTags("Courses")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Courses.StudentCourses, async (ICoursesService service, CancellationToken cancellationToken) =>
            Results.Ok(await service.GetStudentCoursesAsync(cancellationToken)))
            .WithTags("Courses")
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapPost(ApiRoutes.Courses.CoursesList, async (
                CreateCourseRequest request,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            Results.Ok(await service.CreateCourseAsync(request, cancellationToken)))
            .WithTags("Courses")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Courses.Course, async (
                long courseId,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteCourseAsync(courseId, cancellationToken)))
            .WithTags("Courses")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Courses.CourseModules, async (
                long courseId,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            Results.Ok(await service.GetModulesAsync(courseId, cancellationToken)))
            .WithTags("Courses")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        return app;
    }
}
