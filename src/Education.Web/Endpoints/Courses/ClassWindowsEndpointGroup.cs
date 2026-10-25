using Education.Application.ClassWindows;
using Education.Application.Courses;
using Education.Application.Users;
using Education.Contracts;
using Education.Contracts.ClassWindows;
using Education.Domain.Courses;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Маршруты раздела «курсы».</summary>
internal static class ClassWindowsEndpointGroup
{
    /// <summary>Регистрирует маршруты раздела.</summary>
    /// <param name="app">Построитель маршрутов.</param>
    /// <returns>Тот же построитель маршрутов.</returns>
    public static IEndpointRouteBuilder MapClassWindowsEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Courses.ClassWindows, (
                Guid courseId,
                IClassWindowsService service,
                CancellationToken cancellationToken) => ExecuteAsync(async () =>
            {
                var result = await service.GetAsync(courseId, cancellationToken);
                return Results.Ok(new CourseClassWindowsResponse(
                    result.PracticeOnlyDuringClass,
                    result.Groups,
                    result.Windows.Select(ToResponse).ToList()));
            }))
            .WithTags("ClassWindows")
            .WithName("GetCourseClassWindows")
            .WithSummary("Режим курса, группы и пары (преподаватель)")
            .Produces<CourseClassWindowsResponse>()
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Courses.PracticeAccess, (
                Guid courseId,
                SetPracticeAccessRequest request,
                IClassWindowsService service,
                CancellationToken cancellationToken) => ExecuteAsync(async () =>
            {
                await service.SetModeAsync(courseId, request.PracticeOnlyDuringClass, cancellationToken);
                return Results.NoContent();
            }))
            .WithTags("ClassWindows")
            .WithName("SetCoursePracticeAccess")
            .WithSummary("Включить или выключить режим «практика только на парах»")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPost(ApiRoutes.Courses.ClassWindows, (
                Guid courseId,
                StartClassWindowRequest request,
                IClassWindowsService service,
                CancellationToken cancellationToken) => ExecuteAsync(async () =>
            {
                if (String.IsNullOrWhiteSpace(request.GroupName))
                {
                    return Results.Json(
                        new { code = "ClassWindow.GroupRequired", detail = "Группа обязательна." },
                        statusCode: StatusCodes.Status400BadRequest);
                }

                if (request.DurationMinutes is < ClassWindow.MinDurationMinutes or > ClassWindow.MaxDurationMinutes)
                {
                    return Results.Json(
                        new
                        {
                            code = "ClassWindow.InvalidDuration",
                            detail = $"Длительность пары — от {ClassWindow.MinDurationMinutes} до {ClassWindow.MaxDurationMinutes} минут.",
                        },
                        statusCode: StatusCodes.Status400BadRequest);
                }

                var window = await service.StartAsync(courseId, request.GroupName, request.DurationMinutes, cancellationToken);
                return Results.Ok(ToResponse(window));
            }))
            .WithTags("ClassWindows")
            .WithName("StartClassWindow")
            .WithSummary("Начать пару для группы")
            .Produces<ClassWindowResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPost(ApiRoutes.Courses.ClassWindowExtend, (
                Guid windowId,
                ExtendClassWindowRequest request,
                IClassWindowsService service,
                CancellationToken cancellationToken) => ExecuteAsync(async () =>
            {
                if (request.Minutes is < 1 or > ClassWindow.MaxExtendMinutes)
                {
                    return Results.Json(
                        new
                        {
                            code = "ClassWindow.InvalidDuration",
                            detail = $"Продление — от 1 до {ClassWindow.MaxExtendMinutes} минут.",
                        },
                        statusCode: StatusCodes.Status400BadRequest);
                }

                return Results.Ok(ToResponse(await service.ExtendAsync(windowId, request.Minutes, cancellationToken)));
            }))
            .WithTags("ClassWindows")
            .WithName("ExtendClassWindow")
            .WithSummary("Продлить пару")
            .Produces<ClassWindowResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPost(ApiRoutes.Courses.ClassWindowFinish, (
                Guid windowId,
                IClassWindowsService service,
                CancellationToken cancellationToken) => ExecuteAsync(async () =>
                Results.Ok(ToResponse(await service.FinishAsync(windowId, cancellationToken)))))
            .WithTags("ClassWindows")
            .WithName("FinishClassWindow")
            .WithSummary("Завершить пару досрочно")
            .Produces<ClassWindowResponse>()
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Courses.ClassAccess, async (
                Guid courseId,
                IEducationUserResolver userResolver,
                IClassAccessService access,
                ICoursesRepository courses,
                CancellationToken cancellationToken) =>
            {
                var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
                var state = await access.GetStateAsync(courseId, userId, cancellationToken);
                return Results.Ok(new ClassAccessResponse(state.PracticeOnlyDuringClass, state.IsOpen, state.EndsAt));
            })
            .WithTags("ClassWindows")
            .WithName("GetCourseClassAccess")
            .WithSummary("Доступен ли студенту сейчас тест и практика курса")
            .Produces<ClassAccessResponse>()
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        return app;
    }

    private static ClassWindowResponse ToResponse(ClassWindowInfo info) => new(
        info.Id, info.CourseId, info.GroupName, info.StartsAt, info.EndsAt, info.ClosedAt, info.IsActive);

    private static async Task<IResult> ExecuteAsync(Func<Task<IResult>> command)
    {
        try
        {
            return await command();
        }
        catch (CourseAccessDeniedException)
        {
            return Results.Forbid();
        }
        catch (ClassWindowAlreadyOpenException exception)
        {
            return Results.Json(
                new { code = "ClassWindow.AlreadyOpen", detail = exception.Message },
                statusCode: StatusCodes.Status409Conflict);
        }
        catch (ClassWindowAlreadyFinishedException exception)
        {
            return Results.Json(
                new { code = "ClassWindow.AlreadyFinished", detail = exception.Message },
                statusCode: StatusCodes.Status409Conflict);
        }
        catch (ClassWindowNotFoundException exception)
        {
            return Results.Json(
                new { code = "ClassWindow.NotFound", detail = exception.Message },
                statusCode: StatusCodes.Status404NotFound);
        }
        catch (ClassWindowGroupRequiredException exception)
        {
            return Results.Json(
                new { code = "ClassWindow.GroupRequired", detail = exception.Message },
                statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
