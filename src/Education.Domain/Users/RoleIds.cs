namespace Education.Domain.Users;

/// <summary>
/// Предопределенные идентификаторы системных ролей.
/// </summary>
public static class RoleIds
{
    /// <summary>
    /// Идентификатор роли администратора.
    /// </summary>
    public static readonly Guid Admin = Guid.Parse("10000000-0000-0000-0000-000000000001");

    /// <summary>
    /// Идентификатор роли преподавателя.
    /// </summary>
    public static readonly Guid Teacher = Guid.Parse("10000000-0000-0000-0000-000000000002");

    /// <summary>
    /// Идентификатор роли студента.
    /// </summary>
    public static readonly Guid Student = Guid.Parse("10000000-0000-0000-0000-000000000003");
}

