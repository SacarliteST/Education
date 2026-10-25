using Education.Application.Files;
using Education.Application.TaskFiles;
using Education.Contracts.TaskFiles;
using Education.Domain.Practicals;
using Education.Domain.Users;

namespace Education.Web.Endpoints;

/// <summary>Преобразование данных раздела «файлы сдачи заданий» между контрактами API и слоем приложения.</summary>
internal static class TaskFilesEndpointMappings
{
    /// <summary>Преобразует <see cref="IFormFile"/> в команду приложения.</summary>
    /// <param name="file">Исходные данные.</param>
    /// <param name="taskId">Идентификатор задания.</param>
    /// <param name="content">Поток с содержимым файла.</param>
    public static UploadTaskFileCommand ToCommand(this IFormFile file, Guid taskId, Stream content)
    {
        return new UploadTaskFileCommand(
            taskId,
            new UploadFile(file.FileName, file.ContentType, file.Length, content));
    }

    /// <summary>Преобразует <see cref="AddTaskFileCommentRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    /// <param name="taskFileId">Идентификатор файла сдачи.</param>
    public static AddTaskFileCommentCommand ToCommand(this AddTaskFileCommentRequest request, Guid taskFileId)
    {
        return new AddTaskFileCommentCommand(taskFileId, request.Comment);
    }

    /// <summary>Преобразует <see cref="AcceptTaskFileRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    /// <param name="taskFileId">Идентификатор файла сдачи.</param>
    public static AcceptTaskFileCommand ToCommand(this AcceptTaskFileRequest request, Guid taskFileId)
    {
        return new AcceptTaskFileCommand(taskFileId, request.Grade);
    }

    /// <summary>Преобразует <see cref="CaseFile"/> в ответ API.</summary>
    /// <param name="file">Исходные данные.</param>
    public static TaskFileResponse ToResponse(this CaseFile file)
    {
        var comments = file.Comments
            .OrderBy(comment => comment.Id)
            .Select(comment => comment.ToResponse())
            .ToList();

        return new TaskFileResponse(
            file.Id,
            file.Path,
            file.OriginalFileName,
            file.UserId,
            GetFullName(file.User),
            file.IsAccepted,
            file.Comments.OrderBy(comment => comment.Id).LastOrDefault()?.IsGenerated == true,
            file.Grade,
            comments);
    }

    /// <summary>Преобразует <see cref="CaseFileComment"/> в ответ API.</summary>
    /// <param name="comment">Исходные данные.</param>
    public static TaskFileCommentResponse ToResponse(this CaseFileComment comment)
    {
        return new TaskFileCommentResponse(comment.Id, comment.Text, comment.Created, comment.IsGenerated);
    }

    private static string GetFullName(User user)
    {
        return String.Join(
            ' ',
            new[] { user.LastName, user.FirstName, user.MiddleName }.Where(part => !String.IsNullOrWhiteSpace(part)));
    }
}

