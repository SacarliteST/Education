using Education.Application.PracticalModules;

namespace Education.Tests.Support;

/// <summary>Обмен токена без Identity: возвращает предсказуемый токен, в который зашит идентификатор сессии.</summary>
public sealed class FakeTokenExchangeClient : ITokenExchangeClient
{
    /// <inheritdoc />
    public Task<ExchangedToken> ExchangeAsync(
        string audience,
        Guid? sessionId = null,
        DateTimeOffset? sessionExpiresAt = null,
        CancellationToken cancellationToken = default)
        => Task.FromResult(new ExchangedToken(
            "fake-token-" + (sessionId?.ToString("N") ?? "no-session"), 1800));
}
