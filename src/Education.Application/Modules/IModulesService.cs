using Education.Domain.Courses;
using Education.Domain.Materials;

namespace Education.Application.Modules;

/// <summary>
/// Выполняет сценарии работы с модулями.
/// </summary>
public interface IModulesService
{
    /// <summary>
    /// Возвращает модули указанного курса.
    /// </summary>
    Task<IReadOnlyList<Module>> GetModulesAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт модуль в курсе текущего преподавателя.
    /// </summary>
    Task<Module> CreateModuleAsync(CreateModuleCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет модуль из курса текущего преподавателя.
    /// </summary>
    Task DeleteModuleAsync(Guid moduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает теоретические материалы указанного модуля.
    /// </summary>
    Task<IReadOnlyList<TheoreticalMaterial>> GetTheoriesAsync(Guid moduleId, CancellationToken cancellationToken = default);
}

