using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

/// <summary>
/// Сервер-серверный клиент к ручке каталога заданий внешнего модуля
/// (<c>configuration.catalogEndpoint</c>). Реализация — в слое Web (HTTP).
/// </summary>
public interface IModuleCatalogClient
{
    /// <exception cref="ModuleCatalogUnavailableException">Модуль недоступен или ответил неожиданно.</exception>
    Task<IReadOnlyList<ModuleCatalogTask>> GetTasksAsync(
        PracticalModule module,
        CancellationToken cancellationToken = default);
}
