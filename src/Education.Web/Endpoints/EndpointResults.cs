using Education.Application.Courses;

namespace Education.Web.Endpoints;

internal static class EndpointResults
{
    public static async Task<IResult> ExecuteTeacherCommandAsync(Func<Task> command)
    {
        try
        {
            await command();
            return Results.NoContent();
        }
        catch (CourseAccessDeniedException)
        {
            return Results.Forbid();
        }
    }

    public static async Task<IResult> ExecuteTeacherCommandAsync(Func<Task<IResult>> command)
    {
        try
        {
            return await command();
        }
        catch (CourseAccessDeniedException)
        {
            return Results.Forbid();
        }
    }
}
