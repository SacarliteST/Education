using Education.Application.Users;
using Education.Domain.Courses;
using Microsoft.Extensions.Logging;

namespace Education.Application.Courses;

/// <summary>Сценарии работы с курсами преподавателя и студента.</summary>
public sealed class CoursesService : ICoursesService
{
    private readonly IEducationUserResolver userResolver;
    private readonly ICoursesRepository coursesRepository;
    private readonly ILogger<CoursesService> logger;

    /// <summary>Создаёт экземпляр класса <see cref="CoursesService"/>.</summary>
    /// <param name="userResolver">Сопоставление пользователя Identity с учебным пользователем.</param>
    /// <param name="coursesRepository">Хранилище курсов.</param>
    /// <param name="logger">Журнал.</param>
    public CoursesService(
        IEducationUserResolver userResolver,
        ICoursesRepository coursesRepository,
        ILogger<CoursesService> logger)
    {
        this.userResolver = userResolver;
        this.coursesRepository = coursesRepository;
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

        await coursesRepository.DeleteCourseAsync(courseId, cancellationToken);
        logger.LogInformation("Курс {CourseId} удалён преподавателем {LegacyUserId}.", courseId, legacyUserId);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Course>> GetStudentCoursesAsync(CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        return await coursesRepository.GetStudentCoursesAsync(legacyUserId, cancellationToken);
    }
}

