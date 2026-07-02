using Education.Application.Courses;
using Education.Contracts;
using Education.Contracts.Modules;
using Education.Web.Identity;

namespace Education.Web.Endpoints;

public static class ModulesEndpointGroup
{
    public static IEndpointRouteBuilder MapModulesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.Modules.ModulesList, async (
                CreateModuleRequest request,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
                Results.Ok(await service.CreateModuleAsync(request, cancellationToken))))
            .WithTags("Modules")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Modules.Module, async (
                long moduleId,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteModuleAsync(moduleId, cancellationToken)))
            .WithTags("Modules")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Modules.ModuleTheories, async (
                long moduleId,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            Results.Ok(await service.GetTheoriesAsync(moduleId, cancellationToken)))
            .WithTags("Modules")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        return app;
    }
}
