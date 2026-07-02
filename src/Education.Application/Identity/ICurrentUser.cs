namespace Education.Application.Identity;

/// <summary>
/// Предоставляет данные пользователя, связанного с текущим HTTP-запросом.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Идентификатор пользователя во внешнем сервисе идентификации.
    /// </summary>
    Guid UserId { get; }

    /// <summary>
    /// Электронная почта текущего пользователя.
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Отображаемое имя текущего пользователя.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Роли текущего пользователя.
    /// </summary>
    IReadOnlySet<string> Roles { get; }

    /// <summary>
    /// Признак успешной аутентификации текущего пользователя.
    /// </summary>
    bool IsAuthenticated { get; }
}
