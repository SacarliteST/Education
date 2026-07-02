using Education.Application.Files;
using Education.Application.TaskFiles;
using Education.Contracts.TaskFiles;
using Education.Domain.Practicals;
using Education.Domain.Users;

namespace Education.Web.Endpoints;

internal static class TaskFilesEndpointMappings
{
    public static UploadTaskFileCommand ToCommand(this IFormFile file, long taskId, Stream content)
    {
        return new UploadTaskFileCommand(
            taskId,
            new UploadFile(file.FileName, file.ContentType, file.Length, content));
    }

    public static AddTaskFileCommentCommand ToCommand(this AddTaskFileCommentRequest request, long taskFileId)
    {
        return new AddTaskFileCommentCommand(taskFileId, request.Comment);
    }

    public static AcceptTaskFileCommand ToCommand(this AcceptTaskFileRequest request, long taskFileId)
    {
        return new AcceptTaskFileCommand(taskFileId, request.Grade);
    }

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
