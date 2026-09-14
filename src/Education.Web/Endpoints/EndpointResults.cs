using Education.Application.AdminProfiles;
using Education.Application.Courses;
using Education.Application.Files;
using Education.Application.PracticalModules;
using Education.Application.Practicals;
using Education.Application.TestResults;
using FluentValidation;

namespace Education.Web.Endpoints;

internal static class EndpointResults
{
    private const string BadRequestTitle = "Некорректный запрос.";

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
        catch (UnknownStudentsException exception)
        {
            return BadRequestProblem(exception.Message);
        }
        catch (FileStorageValidationException exception)
        {
            return BadRequestProblem(exception.Message);
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
        catch (PracticalModuleNotFoundException exception)
        {
            return Results.Problem(exception.Message, statusCode: StatusCodes.Status404NotFound);
        }
        catch (PracticalHasActivityException exception)
        {
            return Results.Problem(exception.Message, statusCode: StatusCodes.Status409Conflict);
        }
        catch (PracticalQuestionsWeightExceededException exception)
        {
            return BadRequestProblem(exception.Message);
        }
        catch (UnknownStudentsException exception)
        {
            return BadRequestProblem(exception.Message);
        }
        catch (FileStorageValidationException exception)
        {
            return BadRequestProblem(exception.Message);
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
            return BadRequestProblem("Лимит попыток прохождения теста исчерпан.");
        }
        catch (TestAttemptNotFoundException)
        {
            return BadRequestProblem("Активная попытка прохождения теста не найдена.");
        }
        catch (FileStorageValidationException exception)
        {
            return BadRequestProblem(exception.Message);
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
            return BadRequestProblem(exception.Message);
        }
        catch (FileAccessDeniedException)
        {
            return Results.Forbid();
        }
    }

    private static IResult BadRequestProblem(string detail)
    {
        return Results.Problem(
            title: BadRequestTitle,
            detail: detail,
            statusCode: StatusCodes.Status400BadRequest);
    }
}

