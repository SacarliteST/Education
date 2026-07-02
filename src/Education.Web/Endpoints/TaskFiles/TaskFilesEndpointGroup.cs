using Education.Application.TaskFiles;
using Education.Contracts;
using Education.Contracts.TaskFiles;
using Education.Web.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Education.Web.Endpoints;

public static class TaskFilesEndpointGroup
{
    public static IEndpointRouteBuilder MapTaskFilesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.TaskFiles.StudentTaskFile, async (
                long taskId,
                ITaskFilesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
            {
                var file = await service.GetStudentTaskFileAsync(taskId, cancellationToken);
                return file is null ? Results.NotFound() : Results.Ok(file.ToResponse());
            }))
            .WithTags("TaskFiles")
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapPut(ApiRoutes.TaskFiles.StudentTaskFile, async (
                long taskId,
                [FromForm] IFormFile file,
                ITaskFilesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
            {
                await using var stream = file.OpenReadStream();
                var taskFile = await service.UploadStudentTaskFileAsync(file.ToCommand(taskId, stream), cancellationToken);
                return Results.Ok(taskFile.ToResponse());
            }))
            .DisableAntiforgery()
            .WithTags("TaskFiles")
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapGet(ApiRoutes.TaskFiles.TaskFilesByTask, async (
                long taskId,
                ITaskFilesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
                Results.Ok((await service.GetTeacherTaskFilesAsync(taskId, cancellationToken)).Select(file => file.ToResponse()))))
            .WithTags("TaskFiles")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.TaskFiles.PracticalTaskFiles, async (
                long practicalId,
                ITaskFilesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
                Results.Ok((await service.GetTeacherPracticalTaskFilesAsync(practicalId, cancellationToken))
                    .Select(file => file.ToResponse()))))
            .WithTags("TaskFiles")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPost(ApiRoutes.TaskFiles.Comments, async (
                long taskFileId,
                AddTaskFileCommentRequest request,
                IValidator<AddTaskFileCommentRequest> validator,
                ITaskFilesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                var comment = await service.AddTeacherCommentAsync(request.ToCommand(taskFileId), cancellationToken);
                return Results.Ok(comment.ToResponse());
            }))
            .WithTags("TaskFiles")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.TaskFiles.Accept, async (
                long taskFileId,
                AcceptTaskFileRequest request,
                IValidator<AcceptTaskFileRequest> validator,
                ITaskFilesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
            {
                var validation = await EndpointResults.ValidateAsync(validator, request, cancellationToken);
                if (validation is not null)
                {
                    return validation;
                }

                await service.AcceptTaskFileAsync(request.ToCommand(taskFileId), cancellationToken);
                return Results.NoContent();
            }))
            .WithTags("TaskFiles")
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}
