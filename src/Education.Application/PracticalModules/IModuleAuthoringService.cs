namespace Education.Application.PracticalModules;

/// <summary>
/// Выдаёт преподавателю одноразовую ссылку для входа в контур авторинга внешнего
/// модуля без отдельного логина (Token Exchange без сессии + сборка URL).
/// </summary>
public interface IModuleAuthoringService
{
    /// <param name="practicalModuleId">Идентификатор зарегистрированного модуля.</param>
    /// <param name="returnPath">Относительный путь на платформе для кнопки возврата в модуле; <see langword="null"/> — без кнопки.</param>
    /// <param name="taskRef">Ссылка на задание модуля, которое нужно открыть сразу; <see langword="null"/> — стартовая страница.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns><see langword="null"/>, если модуль с таким id не зарегистрирован.</returns>
    /// <exception cref="ModuleDisabledException">Модуль зарегистрирован, но выключен.</exception>
    /// <exception cref="ModulePushFailedException">IdentityService недоступен или отказал в обмене токена.</exception>
    Task<ModuleAuthoringLink?> CreateAuthoringLinkAsync(
        Guid practicalModuleId,
        string? returnPath = null,
        string? taskRef = null,
        CancellationToken cancellationToken = default);
}
