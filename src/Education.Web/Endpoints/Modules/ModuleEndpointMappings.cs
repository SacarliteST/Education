using Education.Application.Modules;
using Education.Contracts.Modules;
using Education.Domain.Courses;

namespace Education.Web.Endpoints;

internal static class ModuleEndpointMappings
{
    public static CreateModuleCommand ToCommand(this CreateModuleRequest request)
    {
        return new CreateModuleCommand(request.CourseId, request.Name);
    }

    public static ModuleResponse ToResponse(this Module module)
    {
        return new ModuleResponse(module.Id, module.Name);
    }
}

