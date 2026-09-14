using Education.Application.Modules;
using Education.Domain.Courses;
using Education.Domain.Materials;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Modules;

internal sealed class EfModulesRepository(EducationDbContext context)
    : RepositoryBase<Module, Guid>(context), IModulesRepository
{
    public override Task<Module?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Modules
            .AsNoTracking()
            .FirstOrDefaultAsync(module => module.Id == id, cancellationToken);
    }

    public Task<bool> IsModuleOwnerAsync(Guid moduleId, Guid teacherUserId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.Modules.AnyAsync(
            module => module.Id == moduleId && module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    public async Task<IReadOnlyList<Module>> GetModulesAsync(
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.Modules
            .AsNoTracking()
            .Where(module => module.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Module> CreateModuleAsync(
        CreateModuleCommand command,
        CancellationToken cancellationToken = default)
    {
        var module = new Module(command.CourseId, command.Name);
        await DatabaseContext.Modules.AddAsync(module, cancellationToken);
        await DatabaseContext.SaveChangesAsync(cancellationToken);

        return module;
    }

    public async Task DeleteModuleAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        await DatabaseContext.Modules
            .Where(module => module.Id == moduleId)
            .ExecuteDeleteAsync(cancellationToken);
    }

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


