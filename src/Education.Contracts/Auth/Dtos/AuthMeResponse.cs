namespace Education.Contracts.Auth;

/// <summary>
/// Данные текущего пользователя, полученные из JWT-токена.
/// </summary>
/// <param name="UserId">Идентификатор пользователя во внешнем identity-сервисе.</param>
/// <param name="Email">Адрес электронной почты пользователя из токена.</param>
/// <param name="Name">Отображаемое имя пользователя из токена.</param>
/// <param name="Roles">Роли пользователя из токена.</param>
/// <param name="IsAuthenticated">Признак успешной аутентификации пользователя.</param>
public sealed record AuthMeResponse(
    Guid UserId,
    string? Email,
    string? Name,
    IReadOnlySet<string> Roles,
    bool IsAuthenticated);


