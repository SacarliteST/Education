using Education.Application.Common;
using Education.Domain.Courses;
using Education.Domain.Materials;

namespace Education.Application.Modules;

/// <summary>
/// Предоставляет операции чтения и записи данных модулей.
/// </summary>
public interface IModulesRepository : IBaseRepository<Module, Guid>
{
    /// <summary>
    /// Проверяет, принадлежит ли модуль курсу указанного преподавателя.
    /// </summary>
    Task<bool> IsModuleOwnerAsync(Guid moduleId, Guid teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает модули указанного курса.
    /// </summary>
    Task<IReadOnlyList<Module>> GetModulesAsync(Guid courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт модуль.
    /// </summary>
    Task<Module> CreateModuleAsync(CreateModuleCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет модуль по идентификатору.
    /// </summary>
    Task DeleteModuleAsync(Guid moduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список теоретических материалов указанного модуля.
    /// </summary>
    Task<IReadOnlyList<TheoreticalMaterial>> GetTheoriesAsync(Guid moduleId, CancellationToken cancellationToken = default);
}

