using Education.Application.Files;
using Education.Contracts;
using Education.Web.Identity;

namespace Education.Web.Endpoints;

/// <summary>Маршруты раздела «файлы».</summary>
internal static class FilesEndpointGroup
{
    /// <summary>Регистрирует маршруты раздела.</summary>
    /// <param name="app">Построитель маршрутов.</param>
    /// <returns>Тот же построитель маршрутов.</returns>
    public static IEndpointRouteBuilder MapFilesEndpointGroup(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Files.Download, async (
                string storageKey,
                IFilesService service,
                CancellationToken cancellationToken) =>
            await EndpointResults.ExecuteFileCommandAsync(async () =>
            {
                var file = await service.DownloadAsync(storageKey, cancellationToken);
                return file is null
                    ? Results.NotFound()
                    : Results.File(file.Content, "application/octet-stream", file.OriginalFileName);
            }))
            .WithTags("Files")
            .WithName("DownloadFile")
            .WithSummary("Скачивание файла")
            .WithDescription("Возвращает файл из хранилища по ключу, если текущий пользователь имеет к нему доступ.")
            .Produces(StatusCodes.Status200OK, contentType: "application/octet-stream")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        return app;
    }
}

