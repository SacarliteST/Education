using Microsoft.Extensions.Logging;

namespace Education.Application.PracticalModules;

/// <inheritdoc />
public sealed class ModuleTrainingService : IModuleTrainingService
{
    private readonly IPracticalModulesRepository repository;
    private readonly ITokenExchangeClient tokenExchangeClient;
    private readonly IModuleIntegrationConfig integrationConfig;
    private readonly ILogger<ModuleTrainingService> logger;

    /// <summary>Создаёт экземпляр класса <see cref="ModuleTrainingService"/>.</summary>
    /// <param name="repository">Реестр внешних практических модулей.</param>
    /// <param name="tokenExchangeClient">Клиент обмена токена для запуска модуля.</param>
    /// <param name="integrationConfig">Настройки интеграции с внешними модулями.</param>
    /// <param name="logger">Журнал.</param>
    public ModuleTrainingService(
        IPracticalModulesRepository repository,
        ITokenExchangeClient tokenExchangeClient,
        IModuleIntegrationConfig integrationConfig,
        ILogger<ModuleTrainingService> logger)
    {
        this.repository = repository;
        this.tokenExchangeClient = tokenExchangeClient;
        this.integrationConfig = integrationConfig;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TrainingModule>> GetTrainingModulesAsync(CancellationToken cancellationToken = default)
    {
        var modules = await repository.GetAllModulesAsync(cancellationToken);
        return modules
            .Where(module => module.IsEnabled && TrainingBankConfiguration.TryRead(module.Configuration, out _))
            .OrderBy(module => module.Name)
            .Select(module => new TrainingModule(module.Id, module.Slug, module.Name, module.Description))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<ModuleTrainingLink?> CreateTrainingLinkAsync(
        Guid practicalModuleId,
        string? returnPath = null,
        CancellationToken cancellationToken = default)
    {
        var module = await repository.GetByIdAsync(practicalModuleId, cancellationToken);
        if (module is null)
        {
            logger.LogWarning("Запрошена ссылка в банк заданий несуществующего модуля {PracticalModuleId}.", practicalModuleId);
            return null;
        }

        if (!module.IsEnabled)
        {
            logger.LogWarning(
                "Отклонена ссылка в банк заданий модуля {ModuleSlug} ({PracticalModuleId}): модуль отключён.",
                module.Slug, practicalModuleId);
            throw new ModuleDisabledException(practicalModuleId);
        }

        if (!TrainingBankConfiguration.TryRead(module.Configuration, out var launchPath))
        {
            logger.LogWarning(
                "Отклонена ссылка в банк заданий модуля {ModuleSlug} ({PracticalModuleId}): банк не включён.",
                module.Slug, practicalModuleId);
            throw new TrainingBankDisabledException(practicalModuleId);
        }

        // Обмен без session_id: токен несёт роль студента, но не открывает платформенную сессию —
        // модуль принимает его как самостоятельную тренировку.
        var exchanged = await tokenExchangeClient.ExchangeAsync(
            module.IdentityAudience, cancellationToken: cancellationToken);

        var url = integrationConfig.BuildTrainingUrl(module.BasePath, launchPath, exchanged.AccessToken, returnPath);
        logger.LogInformation(
            "Выдана ссылка в банк заданий модуля {ModuleSlug} ({PracticalModuleId}), TTL {ExpiresIn} c.",
            module.Slug, practicalModuleId, exchanged.ExpiresIn);
        return new ModuleTrainingLink(url, exchanged.ExpiresIn);
    }
}
