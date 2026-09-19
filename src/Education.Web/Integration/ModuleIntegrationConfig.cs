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

    public string BuildAuthoringUrl(
        string moduleBasePath, string accessToken, string? returnPath = null, string? taskRef = null)
    {
        var (origin, basePath) = ResolveModuleBase(moduleBasePath);
        var query = new List<string>(2);
        if (!String.IsNullOrEmpty(returnPath))
        {
            query.Add("return=" + Uri.EscapeDataString(returnPath));
        }

        if (!String.IsNullOrEmpty(taskRef))
        {
            query.Add("task=" + Uri.EscapeDataString(taskRef));
        }

        var queryString = query.Count == 0 ? String.Empty : "?" + String.Join('&', query);
        return $"{origin}{basePath}/teacher/launch{queryString}#access_token={accessToken}";
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
