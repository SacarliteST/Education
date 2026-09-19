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

    /// <summary>
    /// Заменяет набор студентов, назначенных на курс. Владельца курса проверяет вызывающий сервис.
    /// </summary>
    /// <exception cref="UnknownStudentsException">Часть идентификаторов не сопоставлена с профилем + активной связью.</exception>
    Task SetCourseStudentsAsync(
        SetCourseStudentsCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Заменяет набор студентов, назначенных на практический материал. Владельца проверяет вызывающий сервис.
    /// </summary>
    /// <exception cref="UnknownStudentsException">Часть идентификаторов не сопоставлена с профилем + активной связью.</exception>
    Task SetPracticalStudentsAsync(
        SetPracticalStudentsCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает страницу студентов для назначения на курс. Владельца курса проверяет вызывающий сервис.
    /// </summary>
    Task<AssignableStudentsPage> GetAssignableStudentsPageForCourseAsync(
        Guid courseId,
        AssignableStudentsQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает страницу студентов для назначения на практику. Владельца практики проверяет вызывающий сервис.
    /// </summary>
    Task<AssignableStudentsPage> GetAssignableStudentsPageForPracticalAsync(
        Guid practicalId,
        AssignableStudentsQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Точечно добавляет и убирает студентов курса. Владельца курса проверяет вызывающий сервис.
    /// </summary>
    /// <exception cref="UnknownStudentsException">Часть добавляемых идентификаторов не сопоставлена с профилем + активной связью.</exception>
    Task ChangeCourseStudentsAsync(
        ChangeCourseStudentsCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Точечно добавляет и убирает студентов практики. Владельца практики проверяет вызывающий сервис.
    /// </summary>
    /// <exception cref="UnknownStudentsException">Часть добавляемых идентификаторов не сопоставлена с профилем + активной связью.</exception>
    Task ChangePracticalStudentsAsync(
        ChangePracticalStudentsCommand command,
        CancellationToken cancellationToken = default);
}

