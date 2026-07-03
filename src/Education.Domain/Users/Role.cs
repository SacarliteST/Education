using Education.Domain.Common;

namespace Education.Domain.Users;

/// <summary>
/// Роль пользователя в системе обучения.
/// </summary>
public sealed class Role : Entity
{
    /// <summary>
    /// Название роли.
    /// </summary>
    public string Name { get; private set; } = String.Empty;

    private Role()
    {
    }

    /// <summary>
    /// Создает роль с фиксированным идентификатором.
    /// </summary>
    /// <param name="id">Идентификатор роли.</param>
    /// <param name="name">Название роли.</param>
    public Role(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}

