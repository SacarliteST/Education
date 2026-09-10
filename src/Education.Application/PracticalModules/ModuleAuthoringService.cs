namespace Education.Application.PracticalModules;

/// <inheritdoc />
public sealed class ModuleAuthoringService(
    IPracticalModulesRepository repository,
    ITokenExchangeClient tokenExchangeClient,
    IModuleIntegrationConfig integrationConfig) : IModuleAuthoringService
{
    /// <inheritdoc />
    public async Task<ModuleAuthoringLink?> CreateAuthoringLinkAsync(
        Guid practicalModuleId,
        CancellationToken cancellationToken = default)
    {
        var module = await repository.GetByIdAsync(practicalModuleId, cancellationToken);
        if (module is null)
        {
            return null;
        }

        if (!module.IsEnabled)
        {
            throw new ModuleDisabledException(practicalModuleId);
        }

        // Обмен без session_id: токен несёт роль преподавателя, но не привязан к
        // практической сессии — модуль пускает его в свой teacher-контур по роли.
        var exchanged = await tokenExchangeClient.ExchangeAsync(
            module.IdentityAudience, cancellationToken: cancellationToken);

        var url = integrationConfig.BuildAuthoringUrl(module.BasePath, exchanged.AccessToken);
        return new ModuleAuthoringLink(url, exchanged.ExpiresIn);
    }
}
