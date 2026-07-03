namespace Education.Web.Identity;

/// <summary>
/// Имена политик авторизации Education API.
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>
    /// Политика доступа только для администратора.
    /// </summary>
    public const string AdminOnly = nameof(AdminOnly);

    /// <summary>
    /// Политика доступа только для преподавателя.
    /// </summary>
    public const string TeacherOnly = nameof(TeacherOnly);

    /// <summary>
    /// Политика доступа только для студента.
    /// </summary>
    public const string StudentOnly = nameof(StudentOnly);

    /// <summary>
    /// Политика доступа для любого связанного пользователя Education.
    /// </summary>
    public const string AuthenticatedEducationUser = nameof(AuthenticatedEducationUser);
}

