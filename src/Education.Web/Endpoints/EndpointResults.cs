using Education.Application.Courses;
using Education.Application.Files;
using Education.Application.TestResults;
using FluentValidation;

namespace Education.Web.Endpoints;

internal static class EndpointResults
{
    public static async Task<IResult?> ValidateAsync<TRequest>(
        IValidator<TRequest> validator,
        TRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid)
        {
            return null;
        }

        var errors = validationResult.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        return Results.ValidationProblem(errors);
    }

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
        catch (FileStorageValidationException exception)
        {
            return Results.BadRequest(exception.Message);
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
        catch (FileStorageValidationException exception)
        {
            return Results.BadRequest(exception.Message);
        }
    }

    public static async Task<IResult> ExecuteStudentCommandAsync(Func<Task<IResult>> command)
    {
        try
        {
            return await command();
        }
        catch (StudentPracticalAccessDeniedException)
        {
            return Results.Forbid();
        }
        catch (TestAttemptLimitExceededException)
        {
            return Results.BadRequest();
        }
        catch (TestAttemptNotFoundException)
        {
            return Results.BadRequest();
        }
        catch (FileStorageValidationException exception)
        {
            return Results.BadRequest(exception.Message);
        }
    }

    public static async Task<IResult> ExecuteFileCommandAsync(Func<Task<IResult>> command)
    {
        try
        {
            return await command();
        }
        catch (FileStorageValidationException exception)
        {
            return Results.BadRequest(exception.Message);
        }
        catch (FileAccessDeniedException)
        {
            return Results.Forbid();
        }
    }
}
