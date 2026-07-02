using Education.Application.Files;
using Education.Contracts;
using Education.Web.Identity;

namespace Education.Web.Endpoints;

public static class FilesEndpointGroup
{
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
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedEducationUser);

        return app;
    }
}
