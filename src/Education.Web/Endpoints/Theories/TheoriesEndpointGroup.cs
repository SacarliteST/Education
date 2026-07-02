using Education.Application.Theories;
using Education.Contracts;
using Education.Contracts.Theories;
using Education.Web.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Education.Web.Endpoints;

public static class TheoriesEndpointGroup
{
    public static IEndpointRouteBuilder MapTheoriesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Theories.Theory, async (
                long theoryId,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            {
                var theory = await service.GetTheoryAsync(theoryId, cancellationToken);
                return theory is null ? Results.NotFound() : Results.Ok(theory.ToTextResponse());
            })
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapGet(ApiRoutes.Theories.TheoryDocs, async (
                long theoryId,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetTheoryDocsAsync(theoryId, cancellationToken)).Select(document => document.ToResponse())))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapGet(ApiRoutes.Theories.TheoryLinks, async (
                long theoryId,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetTheoryLinksAsync(theoryId, cancellationToken)).Select(link => link.ToResponse())))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapPost(ApiRoutes.Theories.TheoriesList, async (
                CreateTheoryRequest request,
                IValidator<CreateTheoryRequest> validator,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                return Results.Ok((await service.CreateTheoryAsync(request.ToCommand(), cancellationToken)).ToListItemResponse());
            }))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Theories.TheoryTitle, async (
                long theoryId,
                UpdateTheoryTitleRequest request,
                IValidator<UpdateTheoryTitleRequest> validator,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                await service.UpdateTheoryTitleAsync(theoryId, request.ToCommand(), cancellationToken);
                return Results.NoContent();
            }))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Theories.TheoryText, async (
                long theoryId,
                UpdateTheoryTextRequest request,
                IValidator<UpdateTheoryTextRequest> validator,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                await service.UpdateTheoryTextAsync(theoryId, request.ToCommand(), cancellationToken);
                return Results.NoContent();
            }))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Theories.Theory, async (
                long theoryId,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteTheoryAsync(theoryId, cancellationToken)))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPost(ApiRoutes.Theories.Docs, async (
                [FromForm] long theoryMaterialId,
                [FromForm(Name = "descritpion")] string? legacyDescription,
                [FromForm] string? description,
                IFormFile file,
                IValidator<CreateTheoryDocumentRequest> validator,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var request = new CreateTheoryDocumentRequest(
                    theoryMaterialId,
                    description ?? legacyDescription ?? String.Empty);
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                await using var stream = file.OpenReadStream();
                var command = new CreateTheoryDocumentCommand(
                    request.TheoryMaterialId,
                    request.Description,
                    new TheoryDocumentFile(file.FileName, stream));
                var document = await service.CreateTheoryDocumentAsync(command, cancellationToken);

                return Results.Ok(document.ToResponse());
            }))
            .DisableAntiforgery()
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Theories.Doc, async (
                long docId,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteTheoryDocumentAsync(docId, cancellationToken)))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPost(ApiRoutes.Theories.Links, async (
                CreateTheoryLinkRequest request,
                IValidator<CreateTheoryLinkRequest> validator,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                return Results.Ok((await service.CreateTheoryLinkAsync(request.ToCommand(), cancellationToken)).ToResponse());
            }))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Theories.Link, async (
                long linkId,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteTheoryLinkAsync(linkId, cancellationToken)))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}
