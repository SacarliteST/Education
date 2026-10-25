namespace Education.Application.PracticalModules;

/// <summary>Каталог заданий внешнего практического модуля.</summary>
public sealed class ModuleCatalogService : IModuleCatalogService
{
    private readonly IPracticalModulesRepository repository;
    private readonly IModuleCatalogClient client;

    /// <summary>Создаёт экземпляр класса <see cref="ModuleCatalogService"/>.</summary>
    /// <param name="repository">Реестр внешних практических модулей.</param>
    /// <param name="client">Клиент каталога заданий модуля.</param>
    public ModuleCatalogService(IPracticalModulesRepository repository, IModuleCatalogClient client)
    {
        this.repository = repository;
        this.client = client;
    }

    /// <inheritdoc />
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
