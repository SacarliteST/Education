namespace Education.Application.AdminProfiles;

/// <summary>
/// Предоставляет операции администрирования локальных учебных профилей.
/// </summary>
public interface IAdminProfilesRepository
{
    /// <summary>
    /// Возвращает список локальных учебных профилей.
    /// </summary>
    Task<IReadOnlyList<AdminProfile>> GetProfilesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт локальный учебный профиль и активную связь с identity-сервисом.
    /// </summary>
    Task<AdminProfile> CreateLinkedProfileAsync(
        CreateAdminProfileCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет отображаемые данные локального учебного профиля.
    /// </summary>
    Task<AdminProfile?> UpdateProfileAsync(
        Guid legacyUserId,
        UpdateAdminProfileCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Деактивирует активную связь локального профиля с identity-сервисом.
    /// </summary>
    Task<bool> DeactivateProfileLinkAsync(Guid legacyUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает студентов, доступных для назначения на курс.
    /// </summary>
    Task<IReadOnlyList<AssignableStudent>> GetAssignableStudentsForCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает студентов, доступных для назначения на практический материал.
    /// </summary>
    Task<IReadOnlyList<AssignableStudent>> GetAssignableStudentsForPracticalAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default);
}

