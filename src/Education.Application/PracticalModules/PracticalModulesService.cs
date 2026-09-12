using Education.Application.Audit;
using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

public sealed class PracticalModulesService(
    IPracticalModulesRepository repository,
    IAdminEventRecorder eventRecorder) : IPracticalModulesService
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
        await eventRecorder.RecordAsync(
            AdminEventTypes.ModuleRegistered,
            $"Зарегистрирован модуль «{module.Name}» (slug {module.Slug}, id {module.Id}).",
            cancellationToken);
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
        await eventRecorder.RecordAsync(
            AdminEventTypes.ModuleUpdated,
            $"Изменён модуль «{module.Name}» (slug {module.Slug}, id {module.Id}, включён: {module.IsEnabled}).",
            cancellationToken);
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
        await eventRecorder.RecordAsync(
            AdminEventTypes.ModuleDeleted,
            $"Удалён модуль «{module.Name}» (slug {module.Slug}, id {module.Id}).",
            cancellationToken);
        return true;
    }
}
