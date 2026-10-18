using Education.Application.Courses;
using Education.Application.Practicals;
using Education.Application.TestResults;
using Education.Application.Users;
using Microsoft.Extensions.Logging;

namespace Education.Application.Grades;

public sealed class GradesService(
    IEducationUserResolver userResolver,
    ITestResultsRepository testResultsRepository,
    IGradesRepository gradesRepository,
    IPracticalsRepository practicalsRepository,
    ICoursesRepository coursesRepository,
    ILogger<GradesService> logger)
    : IGradesService
{
    public async Task<PracticalGrade> GetPracticalGradeAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await testResultsRepository.IsPracticalAssignedToStudentAsync(practicalId, userId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в доступе к оценке практики {PracticalId}: студент {UserId} не назначен.",
                practicalId, userId);
            throw new StudentPracticalAccessDeniedException(practicalId);
        }

        return await gradesRepository.GetPracticalGradeAsync(practicalId, userId, cancellationToken);
    }

    public async Task<IReadOnlyList<StudentPracticalGrade>?> GetPracticalStudentGradesAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        var teacherId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await practicalsRepository.IsPracticalOwnerAsync(practicalId, teacherId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в просмотре оценок практики {PracticalId}: преподаватель {UserId} не владелец.",
                practicalId, teacherId);
            return null;
        }

        return await gradesRepository.GetPracticalStudentGradesAsync(practicalId, cancellationToken);
    }

    public async Task<Gradebook> GetCourseGradebookAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var teacherId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsCourseOwnerAsync(courseId, teacherId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в просмотре ведомости курса {CourseId}: преподаватель {UserId} не владелец.",
                courseId, teacherId);
            throw new CourseAccessDeniedException(courseId);
        }

        return await gradesRepository.GetCourseGradebookAsync(courseId, cancellationToken);
    }
}
