using Education.Application.AdminProfiles;
using Education.Application.ClassWindows;
using Education.Application.Courses;
using Education.Application.Files;
using Education.Application.PracticalModules;
using Education.Application.Practicals;
using Education.Application.TestResults;
using FluentValidation;

namespace Education.Web.Endpoints;

/// <summary>Общие обёртки эндпоинтов: валидация запроса и перевод доменных исключений в HTTP-ответы.</summary>
internal static class EndpointResults
{
    private const string BadRequestTitle = "Некорректный запрос.";

    /// <summary>Проверяет запрос валидатором.</summary>
    /// <typeparam name="TRequest">Тип запроса.</typeparam>
    /// <param name="validator">Валидатор запроса.</param>
    /// <param name="request">Проверяемый запрос.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ответ 400 с описанием ошибок или <see langword="null"/>, если запрос корректен.</returns>
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

    /// <summary>Выполняет команду преподавателя без результата и переводит исключения в HTTP-ответы (204 при успехе).</summary>
    /// <param name="command">Выполняемая команда.</param>
    /// <returns>Ответ HTTP.</returns>
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

    /// <summary>Выполняет команду преподавателя и переводит исключения в HTTP-ответы.</summary>
    /// <param name="command">Выполняемая команда, возвращающая ответ.</param>
    /// <returns>Ответ команды или ответ об ошибке.</returns>
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

    /// <summary>Выполняет команду студента и переводит исключения в HTTP-ответы (в том числе закрытое окно пары).</summary>
    /// <param name="command">Выполняемая команда, возвращающая ответ.</param>
    /// <returns>Ответ команды или ответ об ошибке.</returns>
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
        catch (ClassWindowClosedException exception)
        {
            return ClassWindowClosed(exception);
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

    /// <summary>Выполняет операцию с файлом и переводит исключения в HTTP-ответы.</summary>
    /// <param name="command">Выполняемая операция, возвращающая ответ.</param>
    /// <returns>Ответ операции или ответ об ошибке.</returns>
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

    /// <summary>403 с кодом <c>ClassWindow.Closed</c>: практика доступна только во время пары.</summary>
    public static IResult ClassWindowClosed(ClassWindowClosedException exception)
    {
        return Results.Json(
            new { code = "ClassWindow.Closed", detail = exception.Message },
            statusCode: StatusCodes.Status403Forbidden);
    }

    private static IResult BadRequestProblem(string detail)
    {
        return Results.Problem(
            title: BadRequestTitle,
            detail: detail,
            statusCode: StatusCodes.Status400BadRequest);
    }
}

