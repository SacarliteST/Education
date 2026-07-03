using Education.Application.Courses;
using Education.Application.Files;
using Education.Application.Modules;
using Education.Application.Users;
using Education.Domain.Materials;

namespace Education.Application.Theories;

public sealed class TheoriesService(
    IEducationUserResolver userResolver,
    IModulesRepository modulesRepository,
    ITheoriesRepository theoriesRepository,
    IFileStorage fileStorage)
    : ITheoriesService
{
    public Task<TheoreticalMaterial?> GetTheoryAsync(Guid theoryId, CancellationToken cancellationToken = default)
    {
        return theoriesRepository.GetTheoryAsync(theoryId, cancellationToken);
    }

    public Task<IReadOnlyList<TheoreticalMaterialFile>> GetTheoryDocsAsync(Guid theoryId, CancellationToken cancellationToken = default)
    {
        return theoriesRepository.GetTheoryDocsAsync(theoryId, cancellationToken);
    }

    public Task<IReadOnlyList<TheoreticalMaterialLink>> GetTheoryLinksAsync(Guid theoryId, CancellationToken cancellationToken = default)
    {
        return theoriesRepository.GetTheoryLinksAsync(theoryId, cancellationToken);
    }

    public async Task<TheoreticalMaterial> CreateTheoryAsync(CreateTheoryCommand command, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await modulesRepository.IsModuleOwnerAsync(command.ModuleId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(command.ModuleId);
        }

        return await theoriesRepository.CreateTheoryAsync(command, cancellationToken);
    }

    public async Task<TheoreticalMaterialFile> CreateTheoryDocumentAsync(
        CreateTheoryDocumentCommand command,
        CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(command.TheoryMaterialId, cancellationToken);

        var storedFile = await fileStorage.SaveAsync(command.File, cancellationToken);
        return await theoriesRepository.CreateTheoryDocumentAsync(
            command.TheoryMaterialId,
            command.Description,
            storedFile.StorageKey,
            storedFile.OriginalFileName,
            cancellationToken);
    }

    public async Task UpdateTheoryTitleAsync(Guid theoryId, UpdateTheoryTitleCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(theoryId, cancellationToken);
        await theoriesRepository.UpdateTheoryTitleAsync(theoryId, command.Title, cancellationToken);
    }

    public async Task UpdateTheoryTextAsync(Guid theoryId, UpdateTheoryTextCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(theoryId, cancellationToken);
        await theoriesRepository.UpdateTheoryTextAsync(theoryId, command.Text, cancellationToken);
    }

    public async Task DeleteTheoryAsync(Guid theoryId, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(theoryId, cancellationToken);
        await theoriesRepository.DeleteTheoryAsync(theoryId, cancellationToken);
    }

    public async Task DeleteTheoryDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await theoriesRepository.IsTheoryDocumentOwnerAsync(documentId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(documentId);
        }

        var path = await theoriesRepository.GetTheoryDocumentPathAsync(documentId, cancellationToken);
        if (path is not null)
        {
            await fileStorage.DeleteAsync(path, cancellationToken);
        }

        await theoriesRepository.DeleteTheoryDocumentAsync(documentId, cancellationToken);
    }

    public async Task<TheoreticalMaterialLink> CreateTheoryLinkAsync(CreateTheoryLinkCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(command.TheoryMaterialId, cancellationToken);
        return await theoriesRepository.CreateTheoryLinkAsync(command, cancellationToken);
    }

    public async Task DeleteTheoryLinkAsync(Guid linkId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await theoriesRepository.IsTheoryLinkOwnerAsync(linkId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(linkId);
        }

        await theoriesRepository.DeleteTheoryLinkAsync(linkId, cancellationToken);
    }

    private async Task EnsureTheoryOwnerAsync(Guid theoryId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await theoriesRepository.IsTheoryOwnerAsync(theoryId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(theoryId);
        }
    }
}

