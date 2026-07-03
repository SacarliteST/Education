using Education.Application.Courses;
using Education.Application.Users;
using Education.Domain.Courses;
using Education.Domain.Materials;

namespace Education.Application.Modules;

public sealed class ModulesService(
    IEducationUserResolver userResolver,
    ICoursesRepository coursesRepository,
    IModulesRepository modulesRepository)
    : IModulesService
{
    public Task<IReadOnlyList<Module>> GetModulesAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return modulesRepository.GetModulesAsync(courseId, cancellationToken);
    }

    public async Task<Module> CreateModuleAsync(CreateModuleCommand command, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsCourseOwnerAsync(command.CourseId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(command.CourseId);
        }

        return await modulesRepository.CreateModuleAsync(command, cancellationToken);
    }

    public async Task DeleteModuleAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await modulesRepository.IsModuleOwnerAsync(moduleId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(moduleId);
        }

        await modulesRepository.DeleteModuleAsync(moduleId, cancellationToken);
    }

    public Task<IReadOnlyList<TheoreticalMaterial>> GetTheoriesAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return modulesRepository.GetTheoriesAsync(moduleId, cancellationToken);
    }
}

