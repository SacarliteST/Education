namespace Education.Infrastructure.Persistence;

/// <summary>
/// Связь пользователя внешнего identity-сервиса с пользователем учебной системы.
/// </summary>
public sealed class IdentityUserLink
{
    /// <summary>
    /// Идентификатор записи связи.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Идентификатор пользователя в учебной системе.
    /// </summary>
    public Guid LegacyUserId { get; set; }

    /// <summary>
    /// Идентификатор пользователя во внешнем identity-сервисе.
    /// </summary>
    public Guid IdentityUserId { get; set; }

    /// <summary>
    /// Дата создания связи.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Признак активной связи.
    /// </summary>
    public bool IsActive { get; set; }
}


