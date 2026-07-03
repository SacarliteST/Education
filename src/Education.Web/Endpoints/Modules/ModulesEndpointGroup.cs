using Education.Application.Modules;
using Education.Contracts;
using Education.Contracts.Modules;
using Education.Contracts.Theories;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

internal static class ModulesEndpointGroup
{
    public static IEndpointRouteBuilder MapModulesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Modules.ModulesList, async (
                CreateModuleRequest request,
                IValidator<CreateModuleRequest> validator,
                IModulesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                return Results.Ok((await service.CreateModuleAsync(request.ToCommand(), cancellationToken)).ToResponse());
            }))
            .WithTags("Modules")
            .WithName("CreateModule")
            .WithSummary("Создание модуля")
            .WithDescription("Создаёт модуль внутри курса преподавателя.")
            .Produces<ModuleResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Modules.Module, async (
                Guid moduleId,
                IModulesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteModuleAsync(moduleId, cancellationToken)))
            .WithTags("Modules")
            .WithName("DeleteModule")
            .WithSummary("Удаление модуля")
            .WithDescription("Удаляет модуль курса преподавателя.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Modules.ModuleTheories, async (
                Guid moduleId,
                IModulesService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetTheoriesAsync(moduleId, cancellationToken)).Select(theory => theory.ToListItemResponse())))
            .WithTags("Modules")
            .WithName("GetModuleTheories")
            .WithSummary("Получение теории модуля")
            .WithDescription("Возвращает теоретические материалы выбранного модуля.")
            .Produces<IEnumerable<TheoryListItemResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapGet(ApiRoutes.Courses.CourseModules, async (
                Guid courseId,
                IModulesService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetModulesAsync(courseId, cancellationToken)).Select(module => module.ToResponse())))
            .WithTags("Courses")
            .WithName("GetCourseModules")
            .WithSummary("Получение модулей курса")
            .WithDescription("Возвращает модули выбранного курса.")
            .Produces<IEnumerable<ModuleResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        return app;
    }
}

