using Education.Application.PracticalModules;
using Education.Domain.PracticalModules;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.PracticalModules;

internal sealed class EfPracticalModulesRepository(EducationDbContext context)
    : RepositoryBase<PracticalModule, Guid>(context), IPracticalModulesRepository
{
    public override Task<PracticalModule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.PracticalModules.FirstOrDefaultAsync(module => module.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<PracticalModule>> GetAllModulesAsync(CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.PracticalModules
            .AsNoTracking()
            .OrderBy(module => module.Slug)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.PracticalModules.AnyAsync(module => module.Slug == slug, cancellationToken);
    }
}
