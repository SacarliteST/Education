namespace Education.Application.PracticalModules;

/// <summary>Сервер-серверный клиент пуша сессии в модуль. Реализация — в слое Web.</summary>
public interface IModulePushClient
{
    /// <param name="sessionsEndpoint">URL из <c>configuration.sessionsEndpoint</c> модуля.</param>
    /// <param name="moduleSlug">Slug модуля — для выбора <c>X-Service-Key</c> из конфига.</param>
    /// <exception cref="ModulePushFailedException">Модуль недоступен или ответил не 2xx.</exception>
    Task PushAsync(
        string sessionsEndpoint,
        string moduleSlug,
        ModuleSessionPush push,
        CancellationToken cancellationToken = default);
}
