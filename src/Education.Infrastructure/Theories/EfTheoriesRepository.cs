using Education.Application.Theories;
using Education.Domain.Materials;
using Education.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Education.Infrastructure.Theories;

internal sealed class EfTheoriesRepository(EducationDbContext context)
    : RepositoryBase<TheoreticalMaterial, Guid>(context), ITheoriesRepository
{
    public override Task<TheoreticalMaterial?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.TheoreticalMaterials.FirstOrDefaultAsync(theory => theory.Id == id, cancellationToken);
    }

    public Task<bool> IsTheoryOwnerAsync(Guid theoryId, Guid teacherUserId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.TheoreticalMaterials.AnyAsync(
            theory => theory.Id == theoryId && theory.Module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    public Task<bool> IsTheoryLinkOwnerAsync(Guid linkId, Guid teacherUserId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.TheoreticalMaterialLinks.AnyAsync(
            link => link.Id == linkId && link.TheoreticalMaterial.Module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    public Task<bool> IsTheoryDocumentOwnerAsync(Guid documentId, Guid teacherUserId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.TheoreticalMaterialFiles.AnyAsync(
            file => file.Id == documentId && file.TheoreticalMaterial.Module.Course.UserId == teacherUserId,
            cancellationToken);
    }

    public Task<TheoreticalMaterial?> GetTheoryAsync(Guid theoryId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.TheoreticalMaterials
            .AsNoTracking()
            .FirstOrDefaultAsync(theory => theory.Id == theoryId, cancellationToken);
    }

    public async Task<IReadOnlyList<TheoreticalMaterialFile>> GetTheoryDocsAsync(
        Guid theoryId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.TheoreticalMaterialFiles
            .AsNoTracking()
            .Where(file => file.TheoreticalMaterialId == theoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TheoreticalMaterialLink>> GetTheoryLinksAsync(
        Guid theoryId,
        CancellationToken cancellationToken = default)
    {
        return await DatabaseContext.TheoreticalMaterialLinks
            .AsNoTracking()
            .Where(link => link.TheoreticalMaterialId == theoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task<TheoreticalMaterial> CreateTheoryAsync(
        CreateTheoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var theory = new TheoreticalMaterial(command.ModuleId, command.Name, "Текст лекции");
        await DatabaseContext.TheoreticalMaterials.AddAsync(theory, cancellationToken);
        await DatabaseContext.SaveChangesAsync(cancellationToken);

        return theory;
    }

    public async Task<TheoreticalMaterialFile> CreateTheoryDocumentAsync(
        Guid theoryMaterialId,
        string description,
        string path,
        string originalFileName,
        CancellationToken cancellationToken = default)
    {
        var file = new TheoreticalMaterialFile(theoryMaterialId, description, path, originalFileName);
        await DatabaseContext.TheoreticalMaterialFiles.AddAsync(file, cancellationToken);
        await DatabaseContext.SaveChangesAsync(cancellationToken);

        return file;
    }

    public async Task UpdateTheoryTitleAsync(
        Guid theoryId,
        string title,
        CancellationToken cancellationToken = default)
    {
        var theory = await DatabaseContext.TheoreticalMaterials.FirstOrDefaultAsync(
            item => item.Id == theoryId,
            cancellationToken);

        if (theory is null)
        {
            return;
        }

        theory.Rename(title);
        await DatabaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateTheoryTextAsync(
        Guid theoryId,
        string text,
        CancellationToken cancellationToken = default)
    {
        var theory = await DatabaseContext.TheoreticalMaterials.FirstOrDefaultAsync(
            item => item.Id == theoryId,
            cancellationToken);

        if (theory is null)
        {
            return;
        }

        theory.UpdateText(text);
        await DatabaseContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteTheoryAsync(Guid theoryId, CancellationToken cancellationToken = default)
    {
        await DatabaseContext.TheoreticalMaterials
            .Where(theory => theory.Id == theoryId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task<string?> GetTheoryDocumentPathAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        return DatabaseContext.TheoreticalMaterialFiles
            .AsNoTracking()
            .Where(file => file.Id == documentId)
            .Select(file => file.Path)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task DeleteTheoryDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        await DatabaseContext.TheoreticalMaterialFiles
            .Where(file => file.Id == documentId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<TheoreticalMaterialLink> CreateTheoryLinkAsync(
        CreateTheoryLinkCommand command,
        CancellationToken cancellationToken = default)
    {
        var link = new TheoreticalMaterialLink(
            command.TheoryMaterialId,
            command.Description,
            command.Link);

        await DatabaseContext.TheoreticalMaterialLinks.AddAsync(link, cancellationToken);
        await DatabaseContext.SaveChangesAsync(cancellationToken);

        return link;
    }

    public async Task DeleteTheoryLinkAsync(Guid linkId, CancellationToken cancellationToken = default)
    {
        await DatabaseContext.TheoreticalMaterialLinks
            .Where(link => link.Id == linkId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}


