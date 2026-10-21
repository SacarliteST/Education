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
    /// Имя из Identity (claim <c>given_name</c>); <see langword="null"/>, если профиль в Identity не структурирован.
    /// </summary>
    string? GivenName { get; }

    /// <summary>
    /// Фамилия из Identity (claim <c>family_name</c>).
    /// </summary>
    string? FamilyName { get; }

    /// <summary>
    /// Отчество из Identity (claim <c>middle_name</c>).
    /// </summary>
    string? MiddleName { get; }

    /// <summary>
    /// Роли текущего пользователя.
    /// </summary>
    IReadOnlySet<string> Roles { get; }

    /// <summary>
    /// Признак успешной аутентификации текущего пользователя.
    /// </summary>
    bool IsAuthenticated { get; }
}

