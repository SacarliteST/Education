namespace Education.Application.PracticalModules;

/// <summary>Обменянный токен: подписан IdentityService под целевую audience.</summary>
public sealed record ExchangedToken(string AccessToken, int ExpiresIn);
