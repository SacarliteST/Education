using Education.Application.TestResults;
using Education.Application.Users;

namespace Education.Application.Grades;

public sealed class GradesService(
    IEducationUserResolver userResolver,
    ITestResultsRepository testResultsRepository,
    IGradesRepository gradesRepository)
    : IGradesService
{
    public async Task<PracticalGrade> GetPracticalGradeAsync(long practicalId, CancellationToken cancellationToken = default)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await testResultsRepository.IsPracticalAssignedToStudentAsync(practicalId, userId, cancellationToken))
        {
            throw new StudentPracticalAccessDeniedException(practicalId);
        }

        return await gradesRepository.GetPracticalGradeAsync(practicalId, userId, cancellationToken);
    }
}
