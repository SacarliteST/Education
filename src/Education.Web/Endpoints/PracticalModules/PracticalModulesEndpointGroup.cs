using Education.Application.PracticalModules;
using Education.Contracts;
using Education.Contracts.PracticalModules;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

internal static class PracticalModulesEndpointGroup
{
    public static IEndpointRouteBuilder MapPracticalModulesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.PracticalModules.ModulesList, async (
                IPracticalModulesService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetAllAsync(cancellationToken)).Select(module => module.ToResponse())))
            .WithTags("PracticalModules")
            .WithName("GetPracticalModules")
            .WithSummary("Получение реестра практических модулей")
            .WithDescription("Возвращает все зарегистрированные внешние практические модули.")
            .Produces<IEnumerable<PracticalModuleResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.AdminOnly);

        app.MapPost(ApiRoutes.PracticalModules.ModulesList, async (
                CreatePracticalModuleRequest request,
                IValidator<CreatePracticalModuleRequest> validator,
                IPracticalModulesService service,
                CancellationToken cancellationToken) =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                try
                {
                    var module = await service.CreateAsync(request.ToCommand(), cancellationToken);
                    return Results.Created(ApiRoutes.PracticalModules.ForModule(module.Id), module.ToResponse());
                }
                catch (PracticalModuleSlugTakenException)
                {
                    return Results.Conflict();
                }
            })
            .WithTags("PracticalModules")
            .WithName("CreatePracticalModule")
            .WithSummary("Регистрация практического модуля")
            .WithDescription("Добавляет новую запись в реестр внешних практических модулей.")
            .Produces<PracticalModuleResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(AuthorizationPolicies.AdminOnly);

        app.MapPut(ApiRoutes.PracticalModules.Module, async (
                Guid id,
                UpdatePracticalModuleRequest request,
                IValidator<UpdatePracticalModuleRequest> validator,
                IPracticalModulesService service,
                CancellationToken cancellationToken) =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                var module = await service.UpdateAsync(id, request.ToCommand(), cancellationToken);
                return module is null ? Results.NotFound() : Results.Ok(module.ToResponse());
            })
            .WithTags("PracticalModules")
            .WithName("UpdatePracticalModule")
            .WithSummary("Изменение практического модуля")
            .WithDescription("Обновляет запись реестра практического модуля. Slug не меняется.")
            .Produces<PracticalModuleResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.AdminOnly);

        app.MapDelete(ApiRoutes.PracticalModules.Module, async (
                Guid id,
                IPracticalModulesService service,
                CancellationToken cancellationToken) =>
            {
                var deleted = await service.DeleteAsync(id, cancellationToken);
                return deleted ? Results.NoContent() : Results.NotFound();
            })
            .WithTags("PracticalModules")
            .WithName("DeletePracticalModule")
            .WithSummary("Удаление практического модуля")
            .WithDescription("Удаляет запись реестра практического модуля.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.AdminOnly);

        app.MapGet(ApiRoutes.PracticalModules.EnabledModulesList, async (
                IPracticalModulesService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetAllAsync(cancellationToken))
                .Where(module => module.IsEnabled)
                .Select(module => module.ToResponse())))
            .WithTags("PracticalModules")
            .WithName("GetEnabledPracticalModules")
            .WithSummary("Список включённых модулей (для привязки преподавателем)")
            .WithDescription("Только включённые модули. Для пикера привязки внешнего модуля к практике.")
            .Produces<IEnumerable<PracticalModuleResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.PracticalModules.ModuleTasks, async (
                Guid practicalModuleId,
                IModuleCatalogService catalogService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var tasks = await catalogService.GetTasksAsync(practicalModuleId, cancellationToken);
                    return tasks is null
                        ? Results.NotFound()
                        : Results.Ok(tasks.Select(task =>
                            new ModuleTaskResponse(task.Ref, task.Name, task.Description)));
                }
                catch (ModuleCatalogUnavailableException exception)
                {
                    return Results.Problem(exception.Message, statusCode: StatusCodes.Status502BadGateway);
                }
            })
            .WithTags("PracticalModules")
            .WithName("GetPracticalModuleTasks")
            .WithSummary("Каталог заданий внешнего модуля")
            .WithDescription("Сервер-сервер: ядро проксирует ручку каталога модуля. Браузер к API модуля не ходит.")
            .Produces<IEnumerable<ModuleTaskResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status502BadGateway)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}
