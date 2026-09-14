using Education.Application.PracticalModules;
using Microsoft.Extensions.Options;

namespace Education.Web.Integration;

internal sealed class ModuleIntegrationConfig(IOptions<ModuleIntegrationOptions> options) : IModuleIntegrationConfig
{
    private readonly ModuleIntegrationOptions options = options.Value;

    public string BuildReturnUrl(Guid courseId, Guid moduleId, Guid practicalId, Guid sessionId)
    {
        return options.ReturnUrlTemplate
            .Replace("{origin}", options.PlatformOrigin.TrimEnd('/'))
            .Replace("{courseId}", courseId.ToString())
            .Replace("{moduleId}", moduleId.ToString())
            .Replace("{practicalId}", practicalId.ToString())
            .Replace("{sessionId}", sessionId.ToString());
    }

    public string BuildLaunchUrl(string moduleBasePath, Guid sessionId, string accessToken)
    {
        var (origin, basePath) = ResolveModuleBase(moduleBasePath);
        return $"{origin}{basePath}/launch?session={sessionId}#access_token={accessToken}";
    }

    public string BuildAuthoringUrl(string moduleBasePath, string accessToken)
    {
        var (origin, basePath) = ResolveModuleBase(moduleBasePath);
        return $"{origin}{basePath}/teacher/launch#access_token={accessToken}";
    }

    // За общим reverse-proxy: {PlatformOrigin}{basePath}/... (SPA модуля смонтирована в basePath).
    // Безшлюзовая разработка (ModuleWebOrigin задан): SPA модуля отдаётся с корня своего порта,
    // basePath не добавляем.
    private (string Origin, string BasePath) ResolveModuleBase(string moduleBasePath)
    {
        var gatewayless = !String.IsNullOrWhiteSpace(options.ModuleWebOrigin);
        var origin = (gatewayless ? options.ModuleWebOrigin : options.PlatformOrigin).TrimEnd('/');
        var basePath = gatewayless ? String.Empty : "/" + moduleBasePath.Trim('/');
        return (origin, basePath);
    }
}
