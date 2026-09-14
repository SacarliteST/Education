namespace Education.Application.PracticalModules;

/// <summary>Обменянный токен: подписан IdentityService под целевую audience.</summary>
public sealed record ExchangedToken(string AccessToken, int ExpiresIn);

/// <summary>
/// Клиент Token Exchange IdentityService. Берёт токен текущего пользователя (из входящего
/// запроса), обменивает на токен под audience модуля. <c>sessionId</c> попадает в токен
/// claim'ом <c>session_id</c>; <c>null</c> — обмен без сессии (SSO преподавателя в контур
/// авторинга модуля). Реализация — в Web.
/// </summary>
public interface ITokenExchangeClient
{
    /// <exception cref="ModulePushFailedException">IdentityService недоступен или отказал в обмене.</exception>
    Task<ExchangedToken> ExchangeAsync(
        string audience,
        Guid? sessionId = null,
        DateTimeOffset? sessionExpiresAt = null,
        CancellationToken cancellationToken = default);
}
