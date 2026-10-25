using Education.Application.PracticalModules;
using Education.Domain.PracticalModules;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.PracticalModules;

/// <summary>EF Core-реализация хранилища реестра внешних практических модулей.</summary>
internal sealed class EfPracticalModulesRepository : RepositoryBase<PracticalModule, Guid>, IPracticalModulesRepository
{
    /// <summary>Создаёт экземпляр класса <see cref="EfPracticalModulesRepository"/>.</summary>
    /// <param name="context">Контекст базы данных Education.</param>
    public EfPracticalModulesRepository(EducationDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public override Task<PracticalModule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.PracticalModules.FirstOrDefaultAsync(module => module.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PracticalModule>> GetAllModulesAsync(CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.PracticalModules
            .AsNoTracking()
            .OrderBy(module => module.Slug)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.PracticalModules.AnyAsync(module => module.Slug == slug, cancellationToken);
    }
}
