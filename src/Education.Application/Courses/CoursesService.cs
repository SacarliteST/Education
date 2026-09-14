using Education.Application.Users;
using Education.Domain.Courses;
using Microsoft.Extensions.Logging;

namespace Education.Application.Courses;

public sealed class CoursesService(
    IEducationUserResolver userResolver,
    ICoursesRepository coursesRepository,
    ILogger<CoursesService> logger)
    : ICoursesService
{
    public async Task<IReadOnlyList<Course>> GetTeacherCoursesAsync(CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        return await coursesRepository.GetTeacherCoursesAsync(legacyUserId, cancellationToken);
    }

    public async Task<Course> CreateCourseAsync(CreateCourseCommand command, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        var course = await coursesRepository.CreateCourseAsync(legacyUserId, command, cancellationToken);
        logger.LogInformation(
            "Курс {CourseId} «{Name}» создан преподавателем {LegacyUserId}.",
            course.Id, command.Name, legacyUserId);
        return course;
    }

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

    public async Task<IReadOnlyList<Course>> GetStudentCoursesAsync(CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        return await coursesRepository.GetStudentCoursesAsync(legacyUserId, cancellationToken);
    }
}

