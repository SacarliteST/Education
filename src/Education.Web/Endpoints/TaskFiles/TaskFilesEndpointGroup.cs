using Education.Application.TaskFiles;
using Education.Contracts;
using Education.Contracts.TaskFiles;
using Education.Web.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Education.Web.Endpoints;

internal static class TaskFilesEndpointGroup
{
    public static IEndpointRouteBuilder MapTaskFilesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.TaskFiles.StudentTaskFile, async (
                Guid taskId,
                ITaskFilesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteStudentCommandAsync(async () =>
            {
                var file = await service.GetStudentTaskFileAsync(taskId, cancellationToken);
                return file is null ? Results.NotFound() : Results.Ok(file.ToResponse());
            }))
            .WithTags("TaskFiles")
            .WithName("GetStudentTaskFile")
            .WithSummary("Получение файла задания студента")
            .WithDescription("Возвращает файл решения текущего студента по задаче.")
            .Produces<TaskFileResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapPut(ApiRoutes.TaskFiles.StudentTaskFile, async (
                Guid taskId,
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
            .WithName("UploadStudentTaskFile")
            .WithSummary("Загрузка файла задания студентом")
            .WithDescription("Загружает или заменяет файл решения текущего студента по задаче.")
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<TaskFileResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.StudentOnly);

        app.MapGet(ApiRoutes.TaskFiles.TaskFilesByTask, async (
                Guid taskId,
                ITaskFilesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
                Results.Ok((await service.GetTeacherTaskFilesAsync(taskId, cancellationToken)).Select(file => file.ToResponse()))))
            .WithTags("TaskFiles")
            .WithName("GetTeacherTaskFilesByTask")
            .WithSummary("Получение решений по задаче")
            .WithDescription("Возвращает файлы решений студентов по задаче преподавателя.")
            .Produces<IEnumerable<TaskFileResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapGet(ApiRoutes.TaskFiles.PracticalTaskFiles, async (
                Guid practicalId,
                ITaskFilesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteTeacherCommandAsync(async () =>
                Results.Ok((await service.GetTeacherPracticalTaskFilesAsync(practicalId, cancellationToken))
                    .Select(file => file.ToResponse()))))
            .WithTags("TaskFiles")
            .WithName("GetTeacherTaskFilesByPractical")
            .WithSummary("Получение решений по практике")
            .WithDescription("Возвращает файлы решений студентов по всем задачам практики преподавателя.")
            .Produces<IEnumerable<TaskFileResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPost(ApiRoutes.TaskFiles.Comments, async (
                Guid taskFileId,
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
            .WithName("AddTaskFileComment")
            .WithSummary("Добавление комментария к решению")
            .WithDescription("Добавляет комментарий преподавателя к файлу решения студента.")
            .Produces<TaskFileCommentResponse>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        app.MapPut(ApiRoutes.TaskFiles.Accept, async (
                Guid taskFileId,
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
            .WithName("AcceptTaskFile")
            .WithSummary("Приём решения студента")
            .WithDescription("Фиксирует оценку и статус принятия файла решения студента.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.TeacherOnly);

        return app;
    }
}

