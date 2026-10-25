using Education.Application.PracticalModules;
using Education.Contracts.PracticalModules;
using Education.Domain.PracticalModules;

namespace Education.Web.Endpoints;

/// <summary>Преобразование данных раздела «внешние практические модули» между контрактами API и слоем приложения.</summary>
internal static class PracticalModulesEndpointMappings
{
    /// <summary>Преобразует <see cref="CreatePracticalModuleRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
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

    /// <summary>Преобразует <see cref="UpdatePracticalModuleRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
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

    /// <summary>Преобразует <see cref="PracticalModule"/> в ответ API.</summary>
    /// <param name="module">Исходные данные.</param>
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
