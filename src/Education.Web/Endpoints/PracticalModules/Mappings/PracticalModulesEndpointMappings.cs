using Education.Application.PracticalModules;
using Education.Contracts.PracticalModules;
using Education.Domain.PracticalModules;

namespace Education.Web.Endpoints;

internal static class PracticalModulesEndpointMappings
{
    public static CreatePracticalModuleCommand ToCommand(this CreatePracticalModuleRequest request)
    {
        return new CreatePracticalModuleCommand(
            request.Slug.Trim(),
            request.Name.Trim(),
            request.Description.Trim(),
            request.PracticeType.Trim(),
            request.BasePath.Trim(),
            request.IdentityAudience.Trim(),
            request.Configuration?.Trim() is { Length: > 0 } configuration ? configuration : "{}");
    }

    public static UpdatePracticalModuleCommand ToCommand(this UpdatePracticalModuleRequest request)
    {
        return new UpdatePracticalModuleCommand(
            request.Name.Trim(),
            request.Description.Trim(),
            request.PracticeType.Trim(),
            request.BasePath.Trim(),
            request.IdentityAudience.Trim(),
            request.Configuration?.Trim() is { Length: > 0 } configuration ? configuration : "{}",
            request.IsEnabled);
    }

    public static PracticalModuleResponse ToResponse(this PracticalModule module)
    {
        return new PracticalModuleResponse(
            module.Id,
            module.Slug,
            module.Name,
            module.Description,
            module.PracticeType,
            module.BasePath,
            module.IdentityAudience,
            module.IsEnabled,
            module.Configuration);
    }
}
