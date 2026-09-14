using Education.Application.Practicals;
using Education.Contracts;
using Education.Contracts.Practicals;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

internal static class PracticalsEndpointGroup
{
    public static IEndpointRouteBuilder MapPracticalsEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Modules.ModulePracticals, async (
                Guid moduleId,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetPracticalsAsync(moduleId, cancellationToken)).Select(practical => practical.ToResponse())))
            .WithTags("Practicals")
            .WithName("GetModulePracticals")
            .WithSummary("Получение практик модуля")
            .WithDescription("Возвращает практические задания выбранного модуля.")
            .Produces<IEnumerable<PracticalResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapPost(ApiRoutes.Practicals.PracticalsList, async (
                CreatePracticalRequest request,
                IValidator<CreatePracticalRequest> validator,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                return Results.Ok((await service.CreatePracticalAsync(request.ToCommand(), cancellationToken)).ToResponse());
            }))
            .WithTags("Practicals")
            .WithName("CreatePractical")
            .WithSummary("Создание практики")
            .WithDescription("Создаёт практическое задание внутри модуля преподавателя.")
            .Produces<PracticalResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Practicals.Practical, async (
                Guid practicalId,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            {
                var detail = await service.GetDetailAsync(practicalId, cancellationToken);
                return detail is null ? Results.NotFound() : Results.Ok(detail.ToResponse());
            })
            .WithTags("Practicals")
            .WithName("GetPracticalDetail")
            .WithSummary("Детали практики")
            .WithDescription("Вид практики (internal/external), лимиты и привязка к внешнему модулю.")
            .Produces<PracticalDetailResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapPut(ApiRoutes.Practicals.Publish, async (
                Guid practicalId,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.PublishPracticalAsync(practicalId, cancellationToken)))
            .WithTags("Practicals")
            .WithName("PublishPractical")
            .WithSummary("Публикация практики")
            .WithDescription("Открывает практическое задание для назначенных студентов.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Practicals.Practical, async (
                Guid practicalId,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeletePracticalAsync(practicalId, cancellationToken)))
            .WithTags("Practicals")
            .WithName("DeletePractical")
            .WithSummary("Удаление практики")
            .WithDescription("Удаляет практическое задание, если оно принадлежит курсу текущего преподавателя.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Practicals.Tasks, async (
                Guid practicalId,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetTasksAsync(practicalId, cancellationToken)).Select(task => task.ToResponse())))
            .WithTags("Practicals")
            .WithName("GetPracticalTasks")
            .WithSummary("Получение заданий практики")
            .WithDescription("Возвращает задачи, входящие в практическое задание.")
            .Produces<IEnumerable<TaskResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapPost(ApiRoutes.Practicals.Tasks, async (
                Guid practicalId,
                CreateTaskRequest request,
                IValidator<CreateTaskRequest> validator,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                var task = await service.CreateTaskAsync(request.ToCommand(practicalId), cancellationToken);
                return Results.Ok(task.ToResponse());
            }))
            .WithTags("Practicals")
            .WithName("CreatePracticalTask")
            .WithSummary("Создание задания практики")
            .WithDescription("Создаёт задание внутри практического материала текущего преподавателя.")
            .Produces<TaskResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Tasks.Task, async (
                Guid taskId,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            {
                var task = await service.GetTaskAsync(taskId, cancellationToken);
                return task is null ? Results.NotFound() : Results.Ok(task.ToResponse());
            })
            .WithTags("Practicals")
            .WithName("GetTask")
            .WithSummary("Задание по идентификатору")
            .WithDescription("Название и текст задания. Доступно преподавателю и студенту.")
            .Produces<TaskResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapPut(ApiRoutes.Tasks.TaskText, async (
                Guid taskId,
                UpdateTaskTextRequest request,
                IValidator<UpdateTaskTextRequest> validator,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                await service.UpdateTaskTextAsync(taskId, request.ToCommand(), cancellationToken);
                return Results.NoContent();
            }))
            .WithTags("Practicals")
            .WithName("UpdatePracticalTaskText")
            .WithSummary("Обновление текста задания практики")
            .WithDescription("Изменяет текст задания практического материала текущего преподавателя.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Tasks.Task, async (
                Guid taskId,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteTaskAsync(taskId, cancellationToken)))
            .WithTags("Practicals")
            .WithName("DeletePracticalTask")
            .WithSummary("Удаление задания практики")
            .WithDescription("Удаляет задание практического материала текущего преподавателя.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Practicals.Module, async (
                Guid practicalId,
                BindPracticalModuleRequest request,
                IValidator<BindPracticalModuleRequest> validator,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                await service.BindModuleAsync(request.ToCommand(practicalId), cancellationToken);
                return Results.NoContent();
            }))
            .WithTags("Practicals")
            .WithName("BindPracticalModule")
            .WithSummary("Привязка внешнего модуля к практике")
            .WithDescription("Переводит практику в kind=external и связывает её с заданием внешнего модуля (1:1).")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Practicals.Questions, async (
                Guid practicalId,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var setup = await service.GetQuestionsSetupAsync(practicalId, cancellationToken);
                return setup is null ? Results.NotFound() : Results.Ok(setup.ToResponse());
            }))
            .WithTags("Practicals")
            .WithName("GetPracticalQuestionsSetup")
            .WithSummary("Получение настройки вопросов практики")
            .WithDescription("Возвращает вопросы модуля и признак их включения в тест практики.")
            .Produces<PracticalQuestionsSetupResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Practicals.Questions, async (
                Guid practicalId,
                ConfigurePracticalQuestionsRequest request,
                IValidator<ConfigurePracticalQuestionsRequest> validator,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                await service.ConfigureQuestionsAsync(request.ToCommand(practicalId), cancellationToken);
                return Results.NoContent();
            }))
            .WithTags("Practicals")
            .WithName("ConfigurePracticalQuestions")
            .WithSummary("Настройка вопросов практики")
            .WithDescription("Обновляет набор вопросов, используемых в тесте практического задания.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}

