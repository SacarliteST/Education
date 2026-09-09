using System.Text.Json;
using Education.Application.PracticalModules;
using Education.Contracts;
using Education.Contracts.PracticalModules;
using Education.Domain.PracticalModules;
using Education.Web.Identity;

namespace Education.Web.Endpoints;

internal static class ModuleSessionsEndpointGroup
{
    public static IEndpointRouteBuilder MapModuleSessionsEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Practicals.ModuleSessions, async (
                Guid practicalId,
                StartModuleSessionRequest request,
                IModuleSessionsService service,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await service.StartAsync(practicalId, request.TaskId, cancellationToken);
                    return Results.Ok(new StartModuleSessionResponse(
                        result.SessionId, result.LaunchUrl, result.ExpiresAt, result.TryNumber, result.Resumed));
                }
                catch (ExternalTaskNotFoundException)
                {
                    return Results.NotFound();
                }
                catch (TriesExhaustedException)
                {
                    return Results.Json(new { reason = "TriesExhausted" }, statusCode: StatusCodes.Status409Conflict);
                }
                catch (ModulePushFailedException exception)
                {
                    return Results.Json(
                        new { reason = "ModuleUnavailable", detail = exception.Message },
                        statusCode: StatusCodes.Status502BadGateway);
                }
            })
            .WithTags("ModuleSessions")
            .WithName("StartModuleSession")
            .WithSummary("Запуск/продолжение попытки внешнего модуля")
            .Produces<StartModuleSessionResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status502BadGateway)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapGet(ApiRoutes.Practicals.ModuleSessions, async (
                Guid practicalId,
                IModuleSessionsService service,
                CancellationToken cancellationToken) =>
            {
                var sessions = await service.ListForPracticalAsync(practicalId, cancellationToken);
                if (sessions is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(sessions.Select(item => new ModuleSessionSummaryResponse(
                    item.SessionId,
                    item.UserId,
                    item.StudentName,
                    item.TryNumber,
                    item.State.ToWire(),
                    item.EndReason?.ToWire(),
                    item.Grade,
                    item.StartedAt,
                    item.EndedAt)));
            })
            .WithTags("ModuleSessions")
            .WithName("ListPracticalModuleSessions")
            .WithSummary("Список попыток внешнего модуля по практике (преподаватель)")
            .WithDescription("Все попытки студентов по практике. Доступ: преподаватель — владелец курса практики.")
            .Produces<IEnumerable<ModuleSessionSummaryResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Practicals.ModuleSessionCurrent, async (
                Guid practicalId,
                Guid taskId,
                IModuleSessionsService service,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var info = await service.GetCurrentAsync(practicalId, taskId, cancellationToken);
                    return Results.Ok(new CurrentModuleSessionResponse(
                        info.Session is null ? null : ToResponse(info.Session),
                        info.AttemptsCount,
                        info.TriesCount,
                        info.TimeLimitMinutes,
                        info.BestGrade));
                }
                catch (ExternalTaskNotFoundException)
                {
                    return Results.NotFound();
                }
            })
            .WithTags("ModuleSessions")
            .WithName("GetCurrentModuleSession")
            .WithSummary("Гейт кнопок запуска внешней практики")
            .Produces<CurrentModuleSessionResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapGet(ApiRoutes.Practicals.ModuleSession, async (
                Guid practicalId,
                Guid sessionId,
                IModuleSessionsService service,
                CancellationToken cancellationToken) =>
            {
                var status = await service.GetStatusAsync(practicalId, sessionId, cancellationToken);
                return status is null ? Results.NotFound() : Results.Ok(ToResponse(status));
            })
            .WithTags("ModuleSessions")
            .WithName("GetModuleSession")
            .WithSummary("Статус попытки внешнего модуля")
            .Produces<ModuleSessionResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapPost(ApiRoutes.Practicals.ModuleSessionAbandon, async (
                Guid practicalId,
                Guid sessionId,
                IModuleSessionsService service,
                CancellationToken cancellationToken) =>
            {
                var outcome = await service.AbandonAsync(practicalId, sessionId, cancellationToken);
                return outcome switch
                {
                    AbandonOutcome.Abandoned => Results.NoContent(),
                    AbandonOutcome.AlreadyTerminal => Results.Conflict(),
                    _ => Results.NotFound(),
                };
            })
            .WithTags("ModuleSessions")
            .WithName("AbandonModuleSession")
            .WithSummary("Прервать попытку внешнего модуля")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapGet(ApiRoutes.Practicals.ModuleSessionEvents, async (
                Guid practicalId,
                Guid sessionId,
                IModuleSessionsService service,
                CancellationToken cancellationToken) =>
            {
                var events = await service.GetEventsAsync(practicalId, sessionId, cancellationToken);
                if (events is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(events.Select(item => new ModuleSessionEventResponse(
                    item.EventId,
                    item.Kind,
                    item.OccurredAt,
                    JsonDocument.Parse(item.PayloadJson).RootElement.Clone())));
            })
            .WithTags("ModuleSessions")
            .WithName("GetModuleSessionEvents")
            .WithSummary("Лента событий попытки внешнего модуля")
            .WithDescription("«Цифровой след» попытки. Доступ: владелец сессии или преподаватель курса практики.")
            .Produces<IEnumerable<ModuleSessionEventResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapPost(ApiRoutes.PracticalModules.SessionComplete, async (
                Guid sessionId,
                CompleteModuleSessionRequest request,
                HttpContext http,
                IConfiguration configuration,
                IModuleSessionsService service,
                CancellationToken cancellationToken) =>
            {
                if (!IsKnownServiceKey(configuration, http.Request.Headers["X-Service-Key"]))
                {
                    return Results.Unauthorized();
                }

                var outcome = await service.CompleteAsync(
                    sessionId,
                    request.SessionKey,
                    request.Grade,
                    request.CompletionData?.GetRawText(),
                    cancellationToken);

                return outcome switch
                {
                    CompleteOutcome.Accepted => Results.Ok(),
                    CompleteOutcome.AlreadyCompleted => Results.Ok(),
                    CompleteOutcome.SessionClosed => Results.Conflict(),
                    _ => Results.Unauthorized(),
                };
            })
            .WithTags("ModuleSessions")
            .WithName("CompleteModuleSession")
            .WithSummary("Приём оценки от бэкенда модуля")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status409Conflict)
            .AllowAnonymous();

        return app;
    }

    private static bool IsKnownServiceKey(IConfiguration configuration, string? presented)
    {
        if (String.IsNullOrEmpty(presented))
        {
            return false;
        }

        return configuration.GetSection("PracticalModules").GetChildren()
            .Select(module => module["ServiceKey"])
            .Any(key => !String.IsNullOrEmpty(key) && key == presented);
    }

    private static ModuleSessionResponse ToResponse(ModuleSessionStatus status) => new(
        status.SessionId,
        status.State.ToWire(),
        status.TryNumber,
        status.StartedAt,
        status.ExpiresAt,
        status.EndReason?.ToWire(),
        status.Grade,
        status.EndedAt);
}
