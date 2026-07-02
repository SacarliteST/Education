using Education.Application.Users;
using Education.Domain.Courses;

namespace Education.Application.Courses;

public sealed class CoursesService(
    IEducationUserResolver userResolver,
    ICoursesRepository coursesRepository)
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
        return await coursesRepository.CreateCourseAsync(legacyUserId, command, cancellationToken);
    }

    public async Task DeleteCourseAsync(long courseId, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsCourseOwnerAsync(courseId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(courseId);
        }

        await coursesRepository.DeleteCourseAsync(courseId, cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetStudentCoursesAsync(CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        return await coursesRepository.GetStudentCoursesAsync(legacyUserId, cancellationToken);
    }
}
