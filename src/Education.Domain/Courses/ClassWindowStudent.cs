using Education.Domain.Common;

namespace Education.Domain.Courses;

/// <summary>Студент, которому открыта пара вне его группы (точечный доступ к паре).</summary>
public sealed class ClassWindowStudent : Entity
{
    /// <summary>Пара.</summary>
    public Guid ClassWindowId { get; private set; }

    /// <summary>Учебный пользователь, которому открыт доступ.</summary>
    public Guid UserId { get; private set; }

    private ClassWindowStudent()
    {
    }

    /// <summary>Создаёт привязку студента к паре.</summary>
    public static ClassWindowStudent Create(Guid userId) => new() { UserId = userId };
}
