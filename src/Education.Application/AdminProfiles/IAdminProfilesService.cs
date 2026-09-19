namespace Education.Application.AdminProfiles;

/// <summary>
/// Выполняет сценарии администрирования локальных учебных профилей.
/// </summary>
public interface IAdminProfilesService
{
    /// <summary>
    /// Возвращает список локальных учебных профилей.
    /// </summary>
    Task<IReadOnlyList<AdminProfile>> GetProfilesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт локальный учебный профиль и связывает его с identity-пользователем.
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
    /// Деактивирует связь локального профиля с identity-пользователем.
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
    /// Заменяет набор студентов, назначенных на курс.
    /// </summary>
    /// <exception cref="Courses.CourseAccessDeniedException">Курс не принадлежит текущему преподавателю (или не существует).</exception>
    /// <exception cref="UnknownStudentsException">Часть переданных идентификаторов не сопоставлена с привязанным профилем.</exception>
    Task SetCourseStudentsAsync(
        SetCourseStudentsCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Заменяет набор студентов, назначенных на практический материал.
    /// </summary>
    /// <exception cref="Courses.CourseAccessDeniedException">Практика не принадлежит текущему преподавателю (или не существует).</exception>
    /// <exception cref="UnknownStudentsException">Часть переданных идентификаторов не сопоставлена с привязанным профилем.</exception>
    Task SetPracticalStudentsAsync(
        SetPracticalStudentsCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает страницу студентов для назначения на курс с поиском и фильтром по назначению.
    /// </summary>
    /// <exception cref="Courses.CourseAccessDeniedException">Курс не принадлежит текущему преподавателю (или не существует).</exception>
    Task<AssignableStudentsPage> GetAssignableStudentsPageForCourseAsync(
        Guid courseId,
        AssignableStudentsQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает страницу студентов для назначения на практику с поиском и фильтром по назначению.
    /// </summary>
    /// <exception cref="Courses.CourseAccessDeniedException">Практика не принадлежит текущему преподавателю (или не существует).</exception>
    Task<AssignableStudentsPage> GetAssignableStudentsPageForPracticalAsync(
        Guid practicalId,
        AssignableStudentsQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Точечно добавляет и убирает студентов курса, не затрагивая остальных.
    /// </summary>
    /// <exception cref="Courses.CourseAccessDeniedException">Курс не принадлежит текущему преподавателю (или не существует).</exception>
    /// <exception cref="UnknownStudentsException">Часть добавляемых идентификаторов не сопоставлена с привязанным профилем.</exception>
    Task ChangeCourseStudentsAsync(
        ChangeCourseStudentsCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Точечно добавляет и убирает студентов практики, не затрагивая остальных.
    /// </summary>
    /// <exception cref="Courses.CourseAccessDeniedException">Практика не принадлежит текущему преподавателю (или не существует).</exception>
    /// <exception cref="UnknownStudentsException">Часть добавляемых идентификаторов не сопоставлена с привязанным профилем.</exception>
    Task ChangePracticalStudentsAsync(
        ChangePracticalStudentsCommand command,
        CancellationToken cancellationToken = default);
}

