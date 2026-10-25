namespace Education.Application.PracticalModules;

public sealed class ModuleCatalogService(
    IPracticalModulesRepository repository,
    IModuleCatalogClient client) : IModuleCatalogService
{
    public async Task<IReadOnlyList<ModuleCatalogTask>?> GetTasksAsync(
        Guid practicalModuleId,
        CancellationToken cancellationToken = default)
    {
        var module = await repository.GetByIdAsync(practicalModuleId, cancellationToken);
        if (module is null)
        {
            return null;
        }

        return await client.GetTasksAsync(module, cancellationToken);
    }
}
