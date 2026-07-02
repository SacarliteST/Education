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
    public Task<TheoreticalMaterial?> GetTheoryAsync(long theoryId, CancellationToken cancellationToken = default)
    {
        return theoriesRepository.GetTheoryAsync(theoryId, cancellationToken);
    }

    public Task<IReadOnlyList<TheoreticalMaterialFile>> GetTheoryDocsAsync(long theoryId, CancellationToken cancellationToken = default)
    {
        return theoriesRepository.GetTheoryDocsAsync(theoryId, cancellationToken);
    }

    public Task<IReadOnlyList<TheoreticalMaterialLink>> GetTheoryLinksAsync(long theoryId, CancellationToken cancellationToken = default)
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

    public async Task UpdateTheoryTitleAsync(long theoryId, UpdateTheoryTitleCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(theoryId, cancellationToken);
        await theoriesRepository.UpdateTheoryTitleAsync(theoryId, command.Title, cancellationToken);
    }

    public async Task UpdateTheoryTextAsync(long theoryId, UpdateTheoryTextCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(theoryId, cancellationToken);
        await theoriesRepository.UpdateTheoryTextAsync(theoryId, command.Text, cancellationToken);
    }

    public async Task DeleteTheoryAsync(long theoryId, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(theoryId, cancellationToken);
        await theoriesRepository.DeleteTheoryAsync(theoryId, cancellationToken);
    }

    public async Task DeleteTheoryDocumentAsync(long documentId, CancellationToken cancellationToken = default)
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

    public async Task DeleteTheoryLinkAsync(long linkId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await theoriesRepository.IsTheoryLinkOwnerAsync(linkId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(linkId);
        }

        await theoriesRepository.DeleteTheoryLinkAsync(linkId, cancellationToken);
    }

    private async Task EnsureTheoryOwnerAsync(long theoryId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await theoriesRepository.IsTheoryOwnerAsync(theoryId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(theoryId);
        }
    }
}
