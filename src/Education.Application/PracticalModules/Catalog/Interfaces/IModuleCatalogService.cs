namespace Education.Application.PracticalModules;

/// <summary>Отдаёт нормализованный каталог заданий зарегистрированного модуля.</summary>
public interface IModuleCatalogService
{
    /// <returns><see langword="null"/>, если модуль с таким id не зарегистрирован.</returns>
    /// <exception cref="ModuleCatalogUnavailableException">Модуль зарегистрирован, но недоступен.</exception>
    Task<IReadOnlyList<ModuleCatalogTask>?> GetTasksAsync(
        Guid practicalModuleId,
        CancellationToken cancellationToken = default);
}
