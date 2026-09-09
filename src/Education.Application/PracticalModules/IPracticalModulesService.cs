using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

public interface IPracticalModulesService
{
    Task<IReadOnlyList<PracticalModule>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PracticalModule> CreateAsync(CreatePracticalModuleCommand command, CancellationToken cancellationToken = default);

    Task<PracticalModule?> UpdateAsync(
        Guid id,
        UpdatePracticalModuleCommand command,
        CancellationToken cancellationToken = default);

    /// <returns><see langword="false"/>, если модуль не найден.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
