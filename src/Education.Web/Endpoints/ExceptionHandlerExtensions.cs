using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Education.Web.Endpoints;

/// <summary>
/// Глобальный обработчик необработанных исключений — раньше у Education,
/// в отличие от IdentityService и SqlModule, такого не было вообще: любое
/// исключение без явного catch в <see cref="EndpointResults"/> улетало
/// голым 500 без единой строчки в лог (см. SQLTren/PLATFORM.md, задачи
/// по логированию кода, п.1).
/// </summary>
internal static class ExceptionHandlerExtensions
{
    /// <summary>Подключает глобальный обработчик необработанных исключений API.</summary>
    /// <param name="app">Приложение.</param>
    /// <returns>То же приложение.</returns>
    public static WebApplication UseApiExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(exceptionApp =>
        {
            exceptionApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = feature?.Error;

                var statusCode = exception switch
                {
                    BadHttpRequestException badRequest => badRequest.StatusCode,
                    ArgumentException => StatusCodes.Status400BadRequest,
                    Education.Application.Users.EducationUserLinkNotFoundException => StatusCodes.Status403Forbidden,
                    _ => StatusCodes.Status500InternalServerError
                };

                var logger = context.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Education.Web.ExceptionHandler");
                logger.LogError(exception, "Unhandled exception on {Method} {Path}",
                    context.Request.Method, context.Request.Path);

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = statusCode,
                    Title = GetTitle(statusCode),
                    Detail = GetDetail(statusCode)
                });
            });
        });

        return app;
    }

    private static string GetTitle(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "Некорректный запрос",
        StatusCodes.Status403Forbidden => "Доступ запрещён",
        _ => "Внутренняя ошибка сервера"
    };

    private static string GetDetail(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "Проверьте формат и значения параметров запроса.",
        StatusCodes.Status403Forbidden => "Учебный профиль не создан или отключён администратором.",
        _ => "При обработке запроса произошла непредвиденная ошибка."
    };
}
