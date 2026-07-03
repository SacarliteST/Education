using Education.Application.Courses;
using Education.Contracts;
using Education.Contracts.Courses;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

internal static class CoursesEndpointGroup
{
    public static IEndpointRouteBuilder MapCoursesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Courses.TeacherCourses, async (ICoursesService service, CancellationToken cancellationToken) =>
            Results.Ok((await service.GetTeacherCoursesAsync(cancellationToken)).Select(course => course.ToResponse())))
            .WithTags("Courses")
            .WithName("GetTeacherCourses")
            .WithSummary("Получение курсов преподавателя")
            .WithDescription("Возвращает список курсов, доступных текущему преподавателю.")
            .Produces<IEnumerable<CourseResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Courses.StudentCourses, async (ICoursesService service, CancellationToken cancellationToken) =>
            Results.Ok((await service.GetStudentCoursesAsync(cancellationToken)).Select(course => course.ToResponse())))
            .WithTags("Courses")
            .WithName("GetStudentCourses")
            .WithSummary("Получение курсов студента")
            .WithDescription("Возвращает список курсов, назначенных текущему студенту.")
            .Produces<IEnumerable<CourseResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
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
            .WithName("CreateCourse")
            .WithSummary("Создание курса")
            .WithDescription("Создаёт новый курс от имени текущего преподавателя.")
            .Produces<CourseResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Courses.Course, async (
                Guid courseId,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteCourseAsync(courseId, cancellationToken)))
            .WithTags("Courses")
            .WithName("DeleteCourse")
            .WithSummary("Удаление курса")
            .WithDescription("Удаляет курс, если он принадлежит текущему преподавателю.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}

