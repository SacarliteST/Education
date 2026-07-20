using Education.Application.Files;
using Education.Application.Theories;
using Education.Contracts;
using Education.Contracts.Theories;
using Education.Web.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Education.Web.Endpoints;

internal static class TheoriesEndpointGroup
{
    public static IEndpointRouteBuilder MapTheoriesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Theories.Theory, async (
                Guid theoryId,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            {
                var theory = await service.GetTheoryAsync(theoryId, cancellationToken);
                return theory is null ? Results.NotFound() : Results.Ok(theory.ToTextResponse());
            })
            .WithTags("Theories")
            .WithName("GetTheory")
            .WithSummary("Получение текста теории")
            .WithDescription("Возвращает заголовок и текст теоретического материала.")
            .Produces<TheoryTextResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapGet(ApiRoutes.Theories.TheoryDocs, async (
                Guid theoryId,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetTheoryDocsAsync(theoryId, cancellationToken)).Select(document => document.ToResponse())))
            .WithTags("Theories")
            .WithName("GetTheoryDocuments")
            .WithSummary("Получение документов теории")
            .WithDescription("Возвращает документы, прикреплённые к теоретическому материалу.")
            .Produces<IEnumerable<TheoryDocumentResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        app.MapGet(ApiRoutes.Theories.TheoryLinks, async (
                Guid theoryId,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            Results.Ok((await service.GetTheoryLinksAsync(theoryId, cancellationToken)).Select(link => link.ToResponse())))
            .WithTags("Theories")
            .WithName("GetTheoryLinks")
            .WithSummary("Получение ссылок теории")
            .WithDescription("Возвращает внешние ссылки, прикреплённые к теоретическому материалу.")
            .Produces<IEnumerable<TheoryLinkResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
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
            .WithName("CreateTheory")
            .WithSummary("Создание теоретического материала")
            .WithDescription("Создаёт теоретический материал внутри модуля преподавателя.")
            .Produces<TheoryListItemResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Theories.TheoryTitle, async (
                Guid theoryId,
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
            .WithName("UpdateTheoryTitle")
            .WithSummary("Обновление заголовка теории")
            .WithDescription("Изменяет название теоретического материала.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.Theories.TheoryText, async (
                Guid theoryId,
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
            .WithName("UpdateTheoryText")
            .WithSummary("Обновление текста теории")
            .WithDescription("Изменяет текст теоретического материала.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Theories.Theory, async (
                Guid theoryId,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteTheoryAsync(theoryId, cancellationToken)))
            .WithTags("Theories")
            .WithName("DeleteTheory")
            .WithSummary("Удаление теоретического материала")
            .WithDescription("Удаляет теоретический материал преподавателя.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPost(ApiRoutes.Theories.Docs, async (
                [FromForm] Guid theoryMaterialId,
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
                    new UploadFile(file.FileName, file.ContentType, file.Length, stream));
                var document = await service.CreateTheoryDocumentAsync(command, cancellationToken);

                return Results.Ok(document.ToResponse());
            }))
            .DisableAntiforgery()
            .WithTags("Theories")
            .WithName("CreateTheoryDocument")
            .WithSummary("Загрузка документа теории")
            .WithDescription("Добавляет документ к теоретическому материалу преподавателя.")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<TheoryDocumentResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Theories.Doc, async (
                Guid docId,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteTheoryDocumentAsync(docId, cancellationToken)))
            .WithTags("Theories")
            .WithName("DeleteTheoryDocument")
            .WithSummary("Удаление документа теории")
            .WithDescription("Удаляет документ теоретического материала преподавателя.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
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
            .WithName("CreateTheoryLink")
            .WithSummary("Создание ссылки теории")
            .WithDescription("Добавляет внешнюю ссылку к теоретическому материалу преподавателя.")
            .Produces<TheoryLinkResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapDelete(ApiRoutes.Theories.Link, async (
                Guid linkId,
                ITheoriesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(() => service.DeleteTheoryLinkAsync(linkId, cancellationToken)))
            .WithTags("Theories")
            .WithName("DeleteTheoryLink")
            .WithSummary("Удаление ссылки теории")
            .WithDescription("Удаляет внешнюю ссылку теоретического материала преподавателя.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}

