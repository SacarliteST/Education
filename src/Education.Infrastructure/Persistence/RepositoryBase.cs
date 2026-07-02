using Education.Application.Common;
using Education.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Persistence;

internal abstract class RepositoryBase<TEntity, TKey> : IBaseRepository<TEntity, TKey>
    where TEntity : Entity
    where TKey : IEquatable<TKey>
{
    protected readonly EducationDbContext DatabaseContext;

    protected RepositoryBase(EducationDbContext databaseContext)
    {
        DatabaseContext = databaseContext;
    }

    public virtual async Task<PageResult<TEntity>> GetAllAsync(
        Pagination pagination,
        CancellationToken cancellationToken = default)
    {
        var entitySet = DatabaseContext.Set<TEntity>();
        var totalCount = await entitySet.CountAsync(cancellationToken);
        var items = await entitySet
            .OrderBy(entity => entity.Id)
            .Skip(pagination.Offset)
            .Take(pagination.Limit)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PageResult<TEntity>(items, totalCount);
    }

    public virtual async Task CreateAsync(TEntity item, CancellationToken cancellationToken = default)
    {
        await DatabaseContext.Set<TEntity>().AddAsync(item, cancellationToken);
        await DatabaseContext.SaveChangesAsync(cancellationToken);
    }

    public virtual Task UpdateAsync(TEntity item, CancellationToken cancellationToken = default)
    {
        DatabaseContext.Update(item);
        return DatabaseContext.SaveChangesAsync(cancellationToken);
    }

    public abstract Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    public virtual Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        DatabaseContext.Set<TEntity>().Remove(entity);
        return DatabaseContext.SaveChangesAsync(cancellationToken);
    }
}
