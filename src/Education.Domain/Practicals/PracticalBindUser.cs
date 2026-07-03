using Education.Domain.Common;
using Education.Domain.Users;

namespace Education.Domain.Practicals;

/// <summary>
/// Связь практического материала с пользователем, которому он доступен.
/// </summary>
public sealed class PracticalBindUser : Entity
{
    /// <summary>
    /// Идентификатор практического материала.
    /// </summary>
    public Guid PracticalMaterialId { get; private set; }

    /// <summary>
    /// Практический материал, доступный пользователю.
    /// </summary>
    public PracticalMaterial PracticalMaterial { get; private set; } = null!;

    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Пользователь, которому доступен практический материал.
    /// </summary>
    public User User { get; private set; } = null!;

    private PracticalBindUser()
    {
    }

    /// <summary>
    /// Создает связь практического материала с пользователем.
    /// </summary>
    /// <param name="practicalMaterialId">Идентификатор практического материала.</param>
    /// <param name="userId">Идентификатор пользователя.</param>
    public PracticalBindUser(Guid practicalMaterialId, Guid userId)
    {
        PracticalMaterialId = practicalMaterialId;
        UserId = userId;
    }
}

