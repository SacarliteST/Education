using Education.Application.ClassWindows;
using Education.Application.PracticalModules;
using Education.Application.Practicals.TaskAssignments;
using Education.Application.TestResults;
using Education.Contracts;
using Education.Contracts.PracticalModules;
using Education.Contracts.Practicals;
using Education.Domain.PracticalModules;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Маршруты раздела «случайные наборы заданий модуля» (правила выборки и назначения).</summary>
internal static class TaskAssignmentsEndpointGroup
{
    /// <summary>Регистрирует маршруты раздела.</summary>
    /// <param name="app">Построитель маршрутов.</param>
    /// <returns>Тот же построитель маршрутов.</returns>
    public static IEndpointRouteBuilder MapTaskAssignmentsEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Practicals.TaskSelectionRule, async (
                Guid practicalId,
                ITaskSelectionRulesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var rule = await service.GetRuleAsync(practicalId, cancellationToken);
                return rule is null
                    ? Results.NotFound()
                    : Results.Ok(ToResponse(rule, EmptySelectionWarning: false));
            }))
            .WithTags("TaskAssignments")
            .WithName("GetTaskSelectionRule")
            .WithSummary("Правило случайной выборки заданий практики")
            .Produces<TaskSelectionRuleResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Practicals.TaskSelectionRule, async (
                Guid practicalId,
                UpsertTaskSelectionRuleRequest request,
                IValidator<UpsertTaskSelectionRuleRequest> validator,
                ITaskSelectionRulesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                var result = await service.UpsertRuleAsync(
                    practicalId,
                    new UpsertTaskSelectionRuleCommand(
                        request.PracticalModuleId,
                        request.TaskCount,
                        request.DifficultyLevels ?? [],
                        request.TopicRefs ?? []),
                    cancellationToken);
                return Results.Ok(ToResponse(result.Rule, result.EmptySelectionWarning));
            }))
            .WithTags("TaskAssignments")
            .WithName("UpsertTaskSelectionRule")
            .WithSummary("Создание/замена правила случайной выборки")
            .WithDescription(
                "Переводит практику в режим случайной выборки заданий из каталога модуля. " +
                "Несовместимо с ручным списком заданий (409).")
            .Produces<TaskSelectionRuleResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Practicals.TaskSelectionRule, async (
                Guid practicalId,
                ITaskSelectionRulesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var deleted = await service.DeleteRuleAsync(practicalId, cancellationToken);
                return deleted ? Results.NoContent() : Results.NotFound();
            }))
            .WithTags("TaskAssignments")
            .WithName("DeleteTaskSelectionRule")
            .WithSummary("Удаление правила случайной выборки")
            .WithDescription("Возвращает практику в фиксированный режим. 409, если по назначениям уже есть сессии.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Practicals.TaskAssignments, async (
                Guid practicalId,
                Guid classWindowId,
                ITaskSelectionRulesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var rows = await service.ListAssignmentsAsync(practicalId, classWindowId, cancellationToken);
                return Results.Ok(rows.Select(row => new StudentAssignmentsResponse(
                    row.StudentId,
                    row.StudentName,
                    row.Tasks.Select(ToResponse).ToList())));
            }))
            .WithTags("TaskAssignments")
            .WithName("ListTaskAssignments")
            .WithSummary("Таблица «студент → задания» по практике на пару")
            .Produces<IEnumerable<StudentAssignmentsResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPost(ApiRoutes.Practicals.TaskAssignmentRegenerate, async (
                Guid practicalId,
                RegenerateAssignmentsRequest request,
                IValidator<RegenerateAssignmentsRequest> validator,
                ITaskSelectionRulesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                await service.RegenerateAsync(
                    practicalId, request.ClassWindowId, request.StudentId, cancellationToken);
                return Results.NoContent();
            }))
            .WithTags("TaskAssignments")
            .WithName("RegenerateTaskAssignments")
            .WithSummary("Пересбор набора заданий студента (до первой сессии)")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Practicals.MyTaskAssignments, async (
                Guid practicalId,
                ITaskSelectionRulesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
            {
                var info = await service.GetMyAssignmentsAsync(practicalId, cancellationToken);
                return info is null
                    ? Results.NotFound()
                    : Results.Ok(new MyAssignmentsResponse(
                        info.ClassWindowId,
                        info.Tasks.Select(ToResponse).ToList()));
            }))
            .WithTags("TaskAssignments")
            .WithName("GetMyTaskAssignments")
            .WithSummary("Набор заданий текущего студента по практике")
            .WithDescription("404 — правила нет или набор ещё не сформирован (будет сформирован при открытии пары).")
            .Produces<MyAssignmentsResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapPost(ApiRoutes.Practicals.TaskAssignmentStart, async (
                Guid practicalId,
                Guid assignmentId,
                IModuleSessionsService service,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await service.StartByAssignmentAsync(practicalId, assignmentId, cancellationToken);
                    return Results.Ok(new StartModuleSessionResponse(
                        result.SessionId, result.LaunchUrl, result.ExpiresAt, result.TryNumber, result.Resumed));
                }
                catch (ExternalTaskNotFoundException)
                {
                    return Results.NotFound();
                }
                catch (StudentPracticalAccessDeniedException)
                {
                    return Results.Forbid();
                }
                catch (ClassWindowClosedException exception)
                {
                    return EndpointResults.ClassWindowClosed(exception);
                }
                catch (TriesExhaustedException)
                {
                    return Results.Json(new { reason = "TriesExhausted" }, statusCode: StatusCodes.Status409Conflict);
                }
                catch (ModuleSessionClosedException)
                {
                    return Results.Json(new { reason = "SessionClosed" }, statusCode: StatusCodes.Status409Conflict);
                }
                catch (ModulePushFailedException exception)
                {
                    return Results.Json(
                        new { reason = "ModuleUnavailable", detail = exception.Message },
                        statusCode: StatusCodes.Status502BadGateway);
                }
            })
            .WithTags("TaskAssignments")
            .WithName("StartModuleSessionByAssignment")
            .WithSummary("Запуск/продолжение сессии модуля по назначению случайного набора")
            .Produces<StartModuleSessionResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status502BadGateway)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        return app;
    }

    private static TaskSelectionRuleResponse ToResponse(
        TaskSelectionRuleInfo rule, bool EmptySelectionWarning) => new(
        rule.PracticalId,
        rule.PracticalModuleId,
        rule.TaskCount,
        rule.DifficultyLevels,
        rule.TopicRefs,
        EmptySelectionWarning);

    private static AssignmentTaskResponse ToResponse(AssignmentTaskItem item) => new(
        item.AssignmentId,
        item.ExternalTaskRef,
        item.SortOrder,
        item.LastState?.ToWire(),
        item.BestGrade);
}
