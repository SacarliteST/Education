using Education.Application.Audit;
using Education.Contracts;
using Education.Contracts.Audit;
using Education.Web.Identity;

namespace Education.Web.Endpoints;

internal static class AdminEventsEndpointGroup
{
    private const int DefaultTake = 100;
    private const int MaxTake = 500;

    public static IEndpointRouteBuilder MapAdminEventsEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.AdminEvents.EventsList, async (
                int? take,
                IAdminEventsRepository repository,
                CancellationToken cancellationToken) =>
            {
                var effectiveTake = Math.Clamp(take ?? DefaultTake, 1, MaxTake);
                var events = await repository.GetRecentAsync(effectiveTake, cancellationToken);
                return Results.Ok(events.Select(adminEvent => adminEvent.ToResponse()));
            })
            .WithTags("AdminEvents")
            .WithName("GetAdminEvents")
            .WithSummary("Журнал административных действий Education")
            .WithDescription(
                "Последние события — регистрация/изменение модулей, привязка/отвязка профилей. " +
                "Не путать с аудитом IdentityService (логины/роли/блокировки) — это отдельный источник, " +
                "специфичный для Education. По умолчанию 100 записей, максимум 500.")
            .Produces<IEnumerable<AdminEventResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.AdminOnly);

        return app;
    }

    private static AdminEventResponse ToResponse(this Domain.Audit.AdminEvent adminEvent) => new(
        adminEvent.Id,
        adminEvent.ActorIdentityUserId,
        adminEvent.ActorName,
        adminEvent.EventType,
        adminEvent.Description,
        adminEvent.CreatedAt);
}
