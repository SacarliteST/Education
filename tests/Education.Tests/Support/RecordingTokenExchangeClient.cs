using Education.Application.PracticalModules;

namespace Education.Tests.Support;

/// <summary>Обмен токена без Identity: запоминает вызовы, чтобы проверить, с какими параметрами платформа ходила в Identity.</summary>
public sealed class RecordingTokenExchangeClient : ITokenExchangeClient
{
    /// <summary>Токен, который возвращает обмен.</summary>
    public const string Token = "training.jwt.value";

    /// <summary>Имитировать недоступность Identity.</summary>
    public bool Throw { get; init; }

    /// <summary>Все вызовы обмена в порядке выполнения.</summary>
    public List<(string Audience, Guid? SessionId)> Calls { get; } = [];

    /// <inheritdoc />
    public Task<ExchangedToken> ExchangeAsync(
        string audience,
        Guid? sessionId = null,
        DateTimeOffset? sessionExpiresAt = null,
        CancellationToken cancellationToken = default)
    {
        Calls.Add((audience, sessionId));
        if (Throw)
        {
            throw new ModulePushFailedException("test: identity down");
        }

        return Task.FromResult(new ExchangedToken(Token, 1800));
    }
}
