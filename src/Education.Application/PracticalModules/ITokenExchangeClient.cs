namespace Education.Application.PracticalModules;

/// <summary>Обменянный токен: подписан IdentityService под целевую audience, привязан к сессии.</summary>
public sealed record ExchangedToken(string AccessToken, int ExpiresIn);

/// <summary>
/// Клиент Token Exchange IdentityService. Берёт токен текущего пользователя (из входящего
/// запроса), обменивает на токен под audience модуля с claim <c>session_id</c>. Реализация — в Web.
/// </summary>
public interface ITokenExchangeClient
{
    /// <exception cref="ModulePushFailedException">IdentityService недоступен или отказал в обмене.</exception>
    Task<ExchangedToken> ExchangeAsync(
        string audience,
        Guid sessionId,
        DateTimeOffset? sessionExpiresAt,
        CancellationToken cancellationToken = default);
}
