namespace Education.Application.PracticalModules;

/// <summary>Тело пуша сессии бэкенду модуля (<c>POST {module}/module-integration/sessions</c>).</summary>
/// <param name="SessionId">Идентификатор сессии на платформе.</param>
/// <param name="SessionKey">Секрет сессии: модуль подписывает им события и завершение.</param>
/// <param name="UserId">
/// Идентификатор пользователя в identity-сервисе — тот же, что в claim <c>sub</c> обменянного
/// токена. Модуль сверяет владельца сессии именно по нему, НЕ по legacy-id Education.
/// </param>
/// <param name="TaskRef">Ключ задания в модуле.</param>
/// <param name="ReturnUrl">Адрес возврата студента на платформу.</param>
/// <param name="ExpiresAt">Момент, до которого сессия действует; <see langword="null"/> — без ограничения по времени.</param>
/// <param name="MaxAttempts">Лимит отправок решения внутри сессии; <see langword="null"/> — лимит задаёт сам модуль.</param>
public sealed record ModuleSessionPush(
    Guid SessionId,
    string SessionKey,
    Guid UserId,
    string TaskRef,
    string ReturnUrl,
    DateTimeOffset? ExpiresAt,
    int? MaxAttempts = null);
