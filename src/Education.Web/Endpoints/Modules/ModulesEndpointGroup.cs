using Education.Application.Modules;
using Education.Contracts;
using Education.Contracts.Modules;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

public static class ModulesEndpointGroup
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
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Modules.Module, async (
                long moduleId,
                IModulesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteModuleAsync(moduleId, cancellationToken)))
            .WithTags("Modules")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Modules.ModuleTheories, async (
                long moduleId,
                IModulesService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetTheoriesAsync(moduleId, cancellationToken)).Select(theory => theory.ToListItemResponse())))
            .WithTags("Modules")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapGet(ApiRoutes.Courses.CourseModules, async (
                long courseId,
                IModulesService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetModulesAsync(courseId, cancellationToken)).Select(module => module.ToResponse())))
            .WithTags("Courses")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        return app;
    }
}
