using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

/// <summary>Управление реестром внешних практических модулей.</summary>
public interface IPracticalModulesService
{
    /// <summary>Возвращает все зарегистрированные модули.</summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<PracticalModule>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Регистрирует модуль.</summary>
    /// <param name="command">Данные модуля.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Созданный модуль.</returns>
    Task<PracticalModule> CreateAsync(CreatePracticalModuleCommand command, CancellationToken cancellationToken = default);

    /// <summary>Изменяет модуль.</summary>
    /// <param name="id">Идентификатор модуля.</param>
    /// <param name="command">Новые данные модуля.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Изменённый модуль или <see langword="null"/>, если модуль не найден.</returns>
    Task<PracticalModule?> UpdateAsync(
        Guid id,
        UpdatePracticalModuleCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>Удаляет модуль.</summary>
    /// <param name="id">Идентификатор модуля.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns><see langword="false"/>, если модуль не найден.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
