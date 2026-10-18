using Education.Domain.PracticalModules;
using Microsoft.Extensions.Logging;

namespace Education.Application.PracticalModules;

public sealed class PracticalModulesService(
    IPracticalModulesRepository repository,
    ILogger<PracticalModulesService> logger) : IPracticalModulesService
{
    public Task<IReadOnlyList<PracticalModule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetAllModulesAsync(cancellationToken);
    }

    public async Task<PracticalModule> CreateAsync(
        CreatePracticalModuleCommand command,
        CancellationToken cancellationToken = default)
    {
        if (await repository.SlugExistsAsync(command.Slug, cancellationToken))
        {
            logger.LogWarning("Отклонена регистрация модуля: slug «{Slug}» уже занят.", command.Slug);
            throw new PracticalModuleSlugTakenException();
        }

        var module = new PracticalModule(
            command.Slug,
            command.Name,
            command.Description,
            command.PracticeType,
            command.BasePath,
            command.IdentityAudience,
            command.Configuration);

        await repository.CreateAsync(module, cancellationToken);
        logger.LogInformation(
            "Зарегистрирован модуль «{Name}» (slug {Slug}, id {ModuleId}).", module.Name, module.Slug, module.Id);
        return module;
    }

    public async Task<PracticalModule?> UpdateAsync(
        Guid id,
        UpdatePracticalModuleCommand command,
        CancellationToken cancellationToken = default)
    {
        var module = await repository.GetByIdAsync(id, cancellationToken);
        if (module is null)
        {
            return null;
        }

        module.Update(
            command.Name,
            command.Description,
            command.PracticeType,
            command.BasePath,
            command.IdentityAudience,
            command.Configuration,
            command.IsEnabled);

        await repository.UpdateAsync(module, cancellationToken);
        logger.LogInformation(
            "Изменён модуль «{Name}» (slug {Slug}, id {ModuleId}, включён: {IsEnabled}).",
            module.Name, module.Slug, module.Id, module.IsEnabled);
        return module;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var module = await repository.GetByIdAsync(id, cancellationToken);
        if (module is null)
        {
            return false;
        }

        await repository.DeleteAsync(module, cancellationToken);
        logger.LogInformation(
            "Удалён модуль «{Name}» (slug {Slug}, id {ModuleId}).", module.Name, module.Slug, module.Id);
        return true;
    }
}
