using Education.Application.Courses;
using Education.Contracts;
using Education.Contracts.Theories;
using Education.Web.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Education.Web.Endpoints;

public static class TheoriesEndpointGroup
{
    public static IEndpointRouteBuilder MapTheoriesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Theories.Theory, async (
                long theoryId,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            {
                var theory = await service.GetTheoryTextAsync(theoryId, cancellationToken);
                return theory is null ? Results.NotFound() : Results.Ok(theory);
            })
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapGet(ApiRoutes.Theories.TheoryDocs, async (
                long theoryId,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            Results.Ok(await service.GetTheoryDocsAsync(theoryId, cancellationToken)))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapGet(ApiRoutes.Theories.TheoryLinks, async (
                long theoryId,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            Results.Ok(await service.GetTheoryLinksAsync(theoryId, cancellationToken)))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapPost(ApiRoutes.Theories.TheoriesList, async (
                CreateTheoryRequest request,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
                Results.Ok(await service.CreateTheoryAsync(request, cancellationToken))))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Theories.TheoryTitle, async (
                long theoryId,
                UpdateTheoryTitleRequest request,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() =>
                service.UpdateTheoryTitleAsync(theoryId, request, cancellationToken)))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Theories.TheoryText, async (
                long theoryId,
                UpdateTheoryTextRequest request,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() =>
                service.UpdateTheoryTextAsync(theoryId, request, cancellationToken)))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Theories.Theory, async (
                long theoryId,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteTheoryAsync(theoryId, cancellationToken)))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPost(ApiRoutes.Theories.Docs, async (
                [FromForm] long theoryMaterialId,
                [FromForm(Name = "descritpion")] string? legacyDescription,
                [FromForm] string? description,
                IFormFile file,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                await using var stream = file.OpenReadStream();
                var request = new CreateTheoryDocumentRequest(
                    theoryMaterialId,
                    description ?? legacyDescription ?? String.Empty);
                var document = await service.CreateTheoryDocumentAsync(
                    request.TheoryMaterialId,
                    request.Description,
                    new TheoryDocumentFile(file.FileName, stream),
                    cancellationToken);

                return Results.Ok(document);
            }))
            .DisableAntiforgery()
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Theories.Doc, async (
                long docId,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteTheoryDocumentAsync(docId, cancellationToken)))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPost(ApiRoutes.Theories.Links, async (
                CreateTheoryLinkRequest request,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
                Results.Ok(await service.CreateTheoryLinkAsync(request, cancellationToken))))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Theories.Link, async (
                long linkId,
                ICoursesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteTheoryLinkAsync(linkId, cancellationToken)))
            .WithTags("Theories")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}
