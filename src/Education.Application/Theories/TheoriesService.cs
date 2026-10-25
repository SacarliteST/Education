using Education.Application.Courses;
using Education.Application.Files;
using Education.Application.Modules;
using Education.Application.Users;
using Education.Domain.Materials;
using Microsoft.Extensions.Logging;

namespace Education.Application.Theories;

/// <summary>Теоретические материалы модуля: тексты, документы и ссылки.</summary>
public sealed class TheoriesService : ITheoriesService
{
    private readonly IEducationUserResolver userResolver;
    private readonly IModulesRepository modulesRepository;
    private readonly ITheoriesRepository theoriesRepository;
    private readonly IFileStorage fileStorage;
    private readonly ILogger<TheoriesService> logger;

    /// <summary>Создаёт экземпляр класса <see cref="TheoriesService"/>.</summary>
    /// <param name="userResolver">Сопоставление пользователя Identity с учебным пользователем.</param>
    /// <param name="modulesRepository">Хранилище модулей курса.</param>
    /// <param name="theoriesRepository">Хранилище теоретических материалов.</param>
    /// <param name="fileStorage">Файловое хранилище.</param>
    /// <param name="logger">Журнал.</param>
    public TheoriesService(
        IEducationUserResolver userResolver,
        IModulesRepository modulesRepository,
        ITheoriesRepository theoriesRepository,
        IFileStorage fileStorage,
        ILogger<TheoriesService> logger)
    {
        this.userResolver = userResolver;
        this.modulesRepository = modulesRepository;
        this.theoriesRepository = theoriesRepository;
        this.fileStorage = fileStorage;
        this.logger = logger;
    }

    /// <inheritdoc />
    public Task<TheoreticalMaterial?> GetTheoryAsync(Guid theoryId, CancellationToken cancellationToken = default)
    {
        return theoriesRepository.GetTheoryAsync(theoryId, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<TheoreticalMaterialFile>> GetTheoryDocsAsync(Guid theoryId, CancellationToken cancellationToken = default)
    {
        return theoriesRepository.GetTheoryDocsAsync(theoryId, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<TheoreticalMaterialLink>> GetTheoryLinksAsync(Guid theoryId, CancellationToken cancellationToken = default)
    {
        return theoriesRepository.GetTheoryLinksAsync(theoryId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TheoreticalMaterial> CreateTheoryAsync(CreateTheoryCommand command, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await modulesRepository.IsModuleOwnerAsync(command.ModuleId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в создании темы в модуле {ModuleId}: пользователь {LegacyUserId} не владелец.",
                command.ModuleId, legacyUserId);
            throw new CourseAccessDeniedException(command.ModuleId);
        }

        var theory = await theoriesRepository.CreateTheoryAsync(command, cancellationToken);
        logger.LogInformation("Тема {TheoryId} создана в модуле {ModuleId}.", theory.Id, command.ModuleId);
        return theory;
    }

    /// <inheritdoc />
    public async Task<TheoreticalMaterialFile> CreateTheoryDocumentAsync(
        CreateTheoryDocumentCommand command,
        CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(command.TheoryMaterialId, cancellationToken);

        var storedFile = await fileStorage.SaveAsync(command.File, cancellationToken);
        var document = await theoriesRepository.CreateTheoryDocumentAsync(
            command.TheoryMaterialId,
            command.Description,
            storedFile.StorageKey,
            storedFile.OriginalFileName,
            cancellationToken);
        logger.LogInformation(
            "Документ {DocumentId} «{FileName}» добавлен к теме {TheoryId}.",
            document.Id, storedFile.OriginalFileName, command.TheoryMaterialId);
        return document;
    }

    /// <inheritdoc />
    public async Task UpdateTheoryTitleAsync(Guid theoryId, UpdateTheoryTitleCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(theoryId, cancellationToken);
        await theoriesRepository.UpdateTheoryTitleAsync(theoryId, command.Title, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateTheoryTextAsync(Guid theoryId, UpdateTheoryTextCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(theoryId, cancellationToken);
        await theoriesRepository.UpdateTheoryTextAsync(theoryId, command.Text, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteTheoryAsync(Guid theoryId, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(theoryId, cancellationToken);
        await theoriesRepository.DeleteTheoryAsync(theoryId, cancellationToken);
        logger.LogInformation("Тема {TheoryId} удалена.", theoryId);
    }

    /// <inheritdoc />
    public async Task DeleteTheoryDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await theoriesRepository.IsTheoryDocumentOwnerAsync(documentId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в удалении документа {DocumentId}: пользователь {LegacyUserId} не владелец.",
                documentId, legacyUserId);
            throw new CourseAccessDeniedException(documentId);
        }

        var path = await theoriesRepository.GetTheoryDocumentPathAsync(documentId, cancellationToken);
        if (path is not null)
        {
            await fileStorage.DeleteAsync(path, cancellationToken);
        }

        await theoriesRepository.DeleteTheoryDocumentAsync(documentId, cancellationToken);
        logger.LogInformation("Документ {DocumentId} удалён.", documentId);
    }

    /// <inheritdoc />
    public async Task<TheoreticalMaterialLink> CreateTheoryLinkAsync(CreateTheoryLinkCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureTheoryOwnerAsync(command.TheoryMaterialId, cancellationToken);
        return await theoriesRepository.CreateTheoryLinkAsync(command, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteTheoryLinkAsync(Guid linkId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await theoriesRepository.IsTheoryLinkOwnerAsync(linkId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в удалении ссылки {LinkId}: пользователь {LegacyUserId} не владелец.",
                linkId, legacyUserId);
            throw new CourseAccessDeniedException(linkId);
        }

        await theoriesRepository.DeleteTheoryLinkAsync(linkId, cancellationToken);
    }

    private async Task EnsureTheoryOwnerAsync(Guid theoryId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await theoriesRepository.IsTheoryOwnerAsync(theoryId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в доступе к теме {TheoryId}: пользователь {LegacyUserId} не владелец.",
                theoryId, legacyUserId);
            throw new CourseAccessDeniedException(theoryId);
        }
    }
}
