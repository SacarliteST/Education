using Education.Application.Courses;
using Education.Application.Users;
using Education.Domain.Courses;
using Education.Domain.Materials;
using Microsoft.Extensions.Logging;

namespace Education.Application.Modules;

public sealed class ModulesService(
    IEducationUserResolver userResolver,
    ICoursesRepository coursesRepository,
    IModulesRepository modulesRepository,
    ILogger<ModulesService> logger)
    : IModulesService
{
    public Task<IReadOnlyList<Module>> GetModulesAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return modulesRepository.GetModulesAsync(courseId, cancellationToken);
    }

    public Task<Module?> GetModuleAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return modulesRepository.GetByIdAsync(moduleId, cancellationToken);
    }

    public async Task<Module> CreateModuleAsync(CreateModuleCommand command, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsCourseOwnerAsync(command.CourseId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в создании модуля в курсе {CourseId}: пользователь {LegacyUserId} не владелец.",
                command.CourseId, legacyUserId);
            throw new CourseAccessDeniedException(command.CourseId);
        }

        var module = await modulesRepository.CreateModuleAsync(command, cancellationToken);
        logger.LogInformation(
            "Модуль {ModuleId} создан в курсе {CourseId} преподавателем {LegacyUserId}.",
            module.Id, command.CourseId, legacyUserId);
        return module;
    }

    public async Task DeleteModuleAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await modulesRepository.IsModuleOwnerAsync(moduleId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в удалении модуля {ModuleId}: пользователь {LegacyUserId} не владелец.",
                moduleId, legacyUserId);
            throw new CourseAccessDeniedException(moduleId);
        }

        await modulesRepository.DeleteModuleAsync(moduleId, cancellationToken);
        logger.LogInformation("Модуль {ModuleId} удалён преподавателем {LegacyUserId}.", moduleId, legacyUserId);
    }

    public Task<IReadOnlyList<TheoreticalMaterial>> GetTheoriesAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return modulesRepository.GetTheoriesAsync(moduleId, cancellationToken);
    }
}
