namespace Education.Application.PracticalModules;

/// <summary>Тело пуша сессии бэкенду модуля (<c>POST {module}/module-integration/sessions</c>).</summary>
/// <param name="UserId">
/// Идентификатор пользователя в identity-сервисе — тот же, что в claim <c>sub</c> обменянного
/// токена. Модуль сверяет владельца сессии именно по нему, НЕ по legacy-id Education.
/// </param>
public sealed record ModuleSessionPush(
    Guid SessionId,
    string SessionKey,
    Guid UserId,
    string TaskRef,
    string ReturnUrl,
    DateTimeOffset? ExpiresAt);

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
