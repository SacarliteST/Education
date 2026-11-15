using Microsoft.Extensions.Logging;

namespace Education.Application.PracticalModules;

/// <inheritdoc />
public sealed class ModuleAuthoringService : IModuleAuthoringService
{
    private readonly IPracticalModulesRepository repository;
    private readonly IPracticalModuleSessionsRepository sessionsRepository;
    private readonly ITokenExchangeClient tokenExchangeClient;
    private readonly IModuleIntegrationConfig integrationConfig;
    private readonly ILogger<ModuleAuthoringService> logger;

    /// <summary>Создаёт экземпляр класса <see cref="ModuleAuthoringService"/>.</summary>
    /// <param name="repository">Реестр внешних практических модулей.</param>
    /// <param name="sessionsRepository">Хранилище сессий внешних модулей.</param>
    /// <param name="tokenExchangeClient">Клиент обмена токена для запуска модуля.</param>
    /// <param name="integrationConfig">Настройки интеграции с внешними модулями.</param>
    /// <param name="logger">Журнал.</param>
    public ModuleAuthoringService(
        IPracticalModulesRepository repository,
        IPracticalModuleSessionsRepository sessionsRepository,
        ITokenExchangeClient tokenExchangeClient,
        IModuleIntegrationConfig integrationConfig,
        ILogger<ModuleAuthoringService> logger)
    {
        this.repository = repository;
        this.sessionsRepository = sessionsRepository;
        this.tokenExchangeClient = tokenExchangeClient;
        this.integrationConfig = integrationConfig;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<ModuleAuthoringLink?> CreateAuthoringLinkAsync(
        Guid practicalModuleId,
        string? returnPath = null,
        string? taskRef = null,
        Guid? moduleSessionId = null,
        CancellationToken cancellationToken = default)
    {
        var module = await repository.GetByIdAsync(practicalModuleId, cancellationToken);
        if (module is null)
        {
            logger.LogWarning("Запрошена authoring-ссылка на несуществующий модуль {PracticalModuleId}.", practicalModuleId);
            return null;
        }

        if (!module.IsEnabled)
        {
            logger.LogWarning(
                "Отклонена authoring-ссылка на модуль {ModuleSlug} ({PracticalModuleId}): модуль отключён.",
                module.Slug, practicalModuleId);
            throw new ModuleDisabledException(practicalModuleId);
        }

        if (moduleSessionId is { } sessionId
            && !await sessionsRepository.BelongsToModuleAsync(sessionId, practicalModuleId, cancellationToken))
        {
            logger.LogWarning(
                "Отклонена authoring-ссылка: сессия {ModuleSessionId} не относится к модулю {PracticalModuleId}.",
                sessionId,
                practicalModuleId);
            return null;
        }

        // Обмен без session_id: токен несёт роль преподавателя, но не привязан к
        // практической сессии — модуль пускает его в свой teacher-контур по роли.
        var exchanged = await tokenExchangeClient.ExchangeAsync(
            module.IdentityAudience, cancellationToken: cancellationToken);

        var url = integrationConfig.BuildAuthoringUrl(
            module.BasePath,
            exchanged.AccessToken,
            returnPath,
            taskRef,
            moduleSessionId);
        logger.LogInformation(
            "Выдана authoring-ссылка на модуль {ModuleSlug} ({PracticalModuleId}), TTL {ExpiresIn} c.",
            module.Slug, practicalModuleId, exchanged.ExpiresIn);
        return new ModuleAuthoringLink(url, exchanged.ExpiresIn);
    }
}
