namespace Education.Application.Users;

/// <summary>Активного учебного профиля нет (не создан, отключён или автосоздание выключено).</summary>
public sealed class EducationUserLinkNotFoundException(Guid identityUserId)
    : InvalidOperationException($"Active Education profile was not found for user '{identityUserId}'.")
{
    /// <summary>Идентификатор пользователя в Identity.</summary>
    public Guid IdentityUserId { get; } = identityUserId;
}
