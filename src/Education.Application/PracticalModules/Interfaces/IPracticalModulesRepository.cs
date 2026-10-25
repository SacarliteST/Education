using Education.Application.Common;
using Education.Domain.PracticalModules;

namespace Education.Application.PracticalModules;

public interface IPracticalModulesRepository : IBaseRepository<PracticalModule, Guid>
{
    Task<IReadOnlyList<PracticalModule>> GetAllModulesAsync(CancellationToken cancellationToken = default);

    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);
}
