using Education.Application.Practicals.TaskAssignments;
using Education.Domain.Practicals;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Practicals;

/// <summary>EF Core-реализация хранилища правил случайной выборки заданий.</summary>
internal sealed class EfTaskSelectionRulesRepository : ITaskSelectionRulesRepository
{
    private readonly EducationDbContext db;

    /// <summary>Создаёт экземпляр класса <see cref="EfTaskSelectionRulesRepository"/>.</summary>
    /// <param name="db">Контекст базы данных Education.</param>
    public EfTaskSelectionRulesRepository(EducationDbContext db)
    {
        this.db = db;
    }

    /// <inheritdoc />
    public Task<TaskSelectionRule?> GetByPracticalIdAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        return db.TaskSelectionRules
            .FirstOrDefaultAsync(rule => rule.PracticalMaterialId == practicalId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TaskSelectionRule>> ListForCourseAsync(
        Guid courseId, CancellationToken cancellationToken = default)
    {
        return await db.TaskSelectionRules
            .Where(rule => rule.PracticalMaterial.Module.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(TaskSelectionRule rule, CancellationToken cancellationToken = default)
    {
        await db.TaskSelectionRules.AddAsync(rule, cancellationToken);
    }

    /// <inheritdoc />
    public void Remove(TaskSelectionRule rule)
    {
        db.TaskSelectionRules.Remove(rule);
    }

    /// <inheritdoc />
    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => db.SaveChangesAsync(cancellationToken);
}
