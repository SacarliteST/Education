using Education.Application.TestResults;
using Education.Application.Users;
using Microsoft.Extensions.Logging;

namespace Education.Application.Grades;

public sealed class GradesService(
    IEducationUserResolver userResolver,
    ITestResultsRepository testResultsRepository,
    IGradesRepository gradesRepository,
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
}
