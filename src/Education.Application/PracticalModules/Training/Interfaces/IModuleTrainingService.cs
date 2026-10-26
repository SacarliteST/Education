namespace Education.Application.PracticalModules;

/// <summary>Вход студента в открытый банк заданий внешнего модуля.</summary>
public interface IModuleTrainingService
{
    /// <summary>Возвращает включённые модули, у которых открыт банк заданий.</summary>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IReadOnlyList<TrainingModule>> GetTrainingModulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Выдаёт ссылку входа в банк: обмен токена без сессии и сборка адреса. Токен банка не получает прав платформенной сессии.
    /// </summary>
    /// <param name="practicalModuleId">Идентификатор модуля.</param>
    /// <param name="returnPath">Проверенный относительный путь возврата на платформу или <see langword="null"/>.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Ссылка или <see langword="null"/>, если модуль не найден.</returns>
    /// <exception cref="ModuleDisabledException">Модуль выключен.</exception>
    /// <exception cref="TrainingBankDisabledException">Банк у модуля не включён.</exception>
    /// <exception cref="ModulePushFailedException">IdentityService недоступен или отказал в обмене.</exception>
    Task<ModuleTrainingLink?> CreateTrainingLinkAsync(
        Guid practicalModuleId,
        string? returnPath = null,
        CancellationToken cancellationToken = default);
}
