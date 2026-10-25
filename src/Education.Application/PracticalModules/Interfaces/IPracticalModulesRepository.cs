using Education.Application.Common;
using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

/// <summary>Хранилище реестра внешних практических модулей.</summary>
public interface IPracticalModulesRepository : IBaseRepository<PracticalModule, Guid>
{
    /// <summary>Возвращает все зарегистрированные модули.</summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<PracticalModule>> GetAllModulesAsync(CancellationToken cancellationToken = default);

    /// <summary>Проверяет, занят ли slug другим модулем.</summary>
    /// <param name="slug">Проверяемый slug.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns><see langword="true"/>, если модуль с таким slug уже есть.</returns>
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);
}
