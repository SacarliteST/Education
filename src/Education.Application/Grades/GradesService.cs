using Education.Application.Courses;
using Education.Application.Practicals;
using Education.Application.TestResults;
using Education.Application.Users;
using Microsoft.Extensions.Logging;

namespace Education.Application.Grades;

/// <summary>Расчёт оценок по практикам и ведомости курса.</summary>
public sealed class GradesService : IGradesService
{
    private readonly IEducationUserResolver userResolver;
    private readonly ITestResultsRepository testResultsRepository;
    private readonly IGradesRepository gradesRepository;
    private readonly IPracticalsRepository practicalsRepository;
    private readonly ICoursesRepository coursesRepository;
    private readonly ILogger<GradesService> logger;

    /// <summary>Создаёт экземпляр класса <see cref="GradesService"/>.</summary>
    /// <param name="userResolver">Сопоставление пользователя Identity с учебным пользователем.</param>
    /// <param name="testResultsRepository">Хранилище результатов тестов.</param>
    /// <param name="gradesRepository">Хранилище данных для расчёта оценок.</param>
    /// <param name="practicalsRepository">Хранилище практик.</param>
    /// <param name="coursesRepository">Хранилище курсов.</param>
    /// <param name="logger">Журнал.</param>
    public GradesService(
        IEducationUserResolver userResolver,
        ITestResultsRepository testResultsRepository,
        IGradesRepository gradesRepository,
        IPracticalsRepository practicalsRepository,
        ICoursesRepository coursesRepository,
        ILogger<GradesService> logger)
    {
        this.userResolver = userResolver;
        this.testResultsRepository = testResultsRepository;
        this.gradesRepository = gradesRepository;
        this.practicalsRepository = practicalsRepository;
        this.coursesRepository = coursesRepository;
        this.logger = logger;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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
