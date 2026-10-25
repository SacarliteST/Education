using Education.Application.Modules;
using Education.Domain.Courses;
using Education.Domain.Materials;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Modules;

/// <summary>EF Core-реализация хранилища модулей курса.</summary>
internal sealed class EfModulesRepository : RepositoryBase<Module, Guid>, IModulesRepository
{
    /// <summary>Создаёт экземпляр класса <see cref="EfModulesRepository"/>.</summary>
    /// <param name="context">Контекст базы данных Education.</param>
    public EfModulesRepository(EducationDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc />
    public override Task<Module?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Modules
            .AsNoTracking()
            .FirstOrDefaultAsync(module => module.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<bool> IsModuleOwnerAsync(Guid moduleId, Guid teacherUserId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Modules.AnyAsync(
            module => module.Id == moduleId && module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Module>> GetModulesAsync(
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.Modules
            .AsNoTracking()
            .Where(module => module.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Module> CreateModuleAsync(
        CreateModuleCommand command,
        CancellationToken cancellationToken = default)
    {
        var module = new Module(command.CourseId, command.Name);
        await DatabaseContext.Modules.AddAsync(module, cancellationToken);
        await DatabaseContext.SaveChangesAsync(cancellationToken);

        return module;
    }

    /// <inheritdoc />
    public async Task DeleteModuleAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        await DatabaseContext.Modules
            .Where(module => module.Id == moduleId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TheoreticalMaterial>> GetTheoriesAsync(
        Guid moduleId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.TheoreticalMaterials
            .AsNoTracking()
            .Where(theory => theory.ModuleId == moduleId)
            .ToListAsync(cancellationToken);
    }
}


