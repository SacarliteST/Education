namespace Education.Web.Identity;

/// <summary>
/// Роли пользователей, ожидаемые в JWT-токене Education API.
/// </summary>
public static class EducationRoles
{
    /// <summary>
    /// Роль администратора.
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// Роль преподавателя.
    /// </summary>
    public const string Teacher = "Teacher";

    /// <summary>
    /// Роль студента.
    /// </summary>
    public const string Student = "Student";
}

