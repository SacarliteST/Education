using Education.Application.Courses;
using Education.Contracts;
using Education.Contracts.Courses;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

public static class CoursesEndpointGroup
{
    public static IEndpointRouteBuilder MapCoursesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Courses.TeacherCourses, async (ICoursesService service, CancellationToken cancellationToken) =>
            Results.Ok((await service.GetTeacherCoursesAsync(cancellationToken)).Select(course => course.ToResponse())))
            .WithTags("Courses")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Courses.StudentCourses, async (ICoursesService service, CancellationToken cancellationToken) =>
            Results.Ok((await service.GetStudentCoursesAsync(cancellationToken)).Select(course => course.ToResponse())))
            .WithTags("Courses")
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapPost(ApiRoutes.Courses.CoursesList, async (
                CreateCourseRequest request,
                IValidator<CreateCourseRequest> validator,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                return Results.Ok((await service.CreateCourseAsync(request.ToCommand(), cancellationToken)).ToResponse());
            })
            .WithTags("Courses")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Courses.Course, async (
                long courseId,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteCourseAsync(courseId, cancellationToken)))
            .WithTags("Courses")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}
