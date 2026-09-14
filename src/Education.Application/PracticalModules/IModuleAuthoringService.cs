namespace Education.Application.PracticalModules;

/// <summary>
/// Выдаёт преподавателю одноразовую ссылку для входа в контур авторинга внешнего
/// модуля без отдельного логина (Token Exchange без сессии + сборка URL).
/// </summary>
public interface IModuleAuthoringService
{
    /// <returns><see langword="null"/>, если модуль с таким id не зарегистрирован.</returns>
    /// <exception cref="ModuleDisabledException">Модуль зарегистрирован, но выключен.</exception>
    /// <exception cref="ModulePushFailedException">IdentityService недоступен или отказал в обмене токена.</exception>
    Task<ModuleAuthoringLink?> CreateAuthoringLinkAsync(
        Guid practicalModuleId,
        CancellationToken cancellationToken = default);
}
