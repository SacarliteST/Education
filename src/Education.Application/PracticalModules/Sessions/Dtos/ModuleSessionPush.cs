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
    DateTimeOffset? ExpiresAt,
    int? MaxAttempts = null);
