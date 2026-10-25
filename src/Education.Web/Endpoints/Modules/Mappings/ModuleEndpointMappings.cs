using Education.Application.Modules;
using Education.Contracts.Modules;
using Education.Domain.Courses;

namespace Education.Web.Endpoints;

/// <summary>Преобразование данных раздела «модули курса» между контрактами API и слоем приложения.</summary>
internal static class ModuleEndpointMappings
{
    /// <summary>Преобразует <see cref="CreateModuleRequest"/> в команду приложения.</summary>
    /// <param name="request">Исходные данные.</param>
    public static CreateModuleCommand ToCommand(this CreateModuleRequest request)
    {
        return new CreateModuleCommand(request.CourseId, request.Name);
    }

    /// <summary>Преобразует <see cref="Module"/> в ответ API.</summary>
    /// <param name="module">Исходные данные.</param>
    public static ModuleResponse ToResponse(this Module module)
    {
        return new ModuleResponse(module.Id, module.Name);
    }
}

