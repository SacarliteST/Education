using Education.Application.PracticalModules;
using Education.Application.Practicals;
using Education.Application.Users;
using Education.Domain.Courses;
using Microsoft.Extensions.Logging;

namespace Education.Application.Courses;

/// <summary>Сценарии работы с курсами преподавателя и студента.</summary>
public sealed class CoursesService : ICoursesService
{
    private readonly IEducationUserResolver userResolver;
    private readonly ICoursesRepository coursesRepository;
    private readonly IPracticalModuleSessionsRepository moduleSessionsRepository;
    private readonly ILogger<CoursesService> logger;

    /// <summary>Создаёт экземпляр класса <see cref="CoursesService"/>.</summary>
    /// <param name="userResolver">Сопоставление пользователя Identity с учебным пользователем.</param>
    /// <param name="coursesRepository">Хранилище курсов.</param>
    /// <param name="moduleSessionsRepository">Хранилище сессий внешних модулей.</param>
    /// <param name="logger">Журнал.</param>
    public CoursesService(
        IEducationUserResolver userResolver,
        ICoursesRepository coursesRepository,
        IPracticalModuleSessionsRepository moduleSessionsRepository,
        ILogger<CoursesService> logger)
    {
        this.userResolver = userResolver;
        this.coursesRepository = coursesRepository;
        this.moduleSessionsRepository = moduleSessionsRepository;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Course>> GetTeacherCoursesAsync(CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        return await coursesRepository.GetTeacherCoursesAsync(legacyUserId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Course> CreateCourseAsync(CreateCourseCommand command, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        var course = await coursesRepository.CreateCourseAsync(legacyUserId, command, cancellationToken);
        logger.LogInformation(
            "Курс {CourseId} «{Name}» создан преподавателем {LegacyUserId}.",
            course.Id, command.Name, legacyUserId);
        return course;
    }

    /// <inheritdoc />
    public async Task DeleteCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsCourseOwnerAsync(courseId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в удалении курса {CourseId}: пользователь {LegacyUserId} не владелец.",
                courseId, legacyUserId);
            throw new CourseAccessDeniedException(courseId);
        }

        if (await moduleSessionsRepository.HasActiveForCourseAsync(courseId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в удалении курса {CourseId}: есть активные сессии модуля.", courseId);
            throw new ActiveModuleSessionsException("курс");
        }

        await coursesRepository.DeleteCourseAsync(courseId, cancellationToken);
        logger.LogInformation(
            "Курс {CourseId} удалён преподавателем {LegacyUserId} (мягкое удаление).", courseId, legacyUserId);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Course>> GetStudentCoursesAsync(CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        return await coursesRepository.GetStudentCoursesAsync(legacyUserId, cancellationToken);
    }
}

