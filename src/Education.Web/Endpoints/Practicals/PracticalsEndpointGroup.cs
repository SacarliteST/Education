using Education.Application.Practicals;
using Education.Contracts;
using Education.Contracts.Practicals;
using Education.Web.Identity;
using FluentValidation;

namespace Education.Web.Endpoints;

public static class PracticalsEndpointGroup
{
    public static IEndpointRouteBuilder MapPracticalsEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Modules.ModulePracticals, async (
                long moduleId,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetPracticalsAsync(moduleId, cancellationToken)).Select(practical => practical.ToResponse())))
            .WithTags("Practicals")
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
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Practicals.Publish, async (
                long practicalId,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.PublishPracticalAsync(practicalId, cancellationToken)))
            .WithTags("Practicals")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.Practicals.Tasks, async (
                long practicalId,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetTasksAsync(practicalId, cancellationToken)).Select(task => task.ToResponse())))
            .WithTags("Practicals")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapGet(ApiRoutes.Practicals.Questions, async (
                long practicalId,
                IPracticalsService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var setup = await service.GetQuestionsSetupAsync(practicalId, cancellationToken);
                return setup is null ? Results.NotFound() : Results.Ok(setup.ToResponse());
            }))
            .WithTags("Practicals")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Practicals.Questions, async (
                long practicalId,
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
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}
