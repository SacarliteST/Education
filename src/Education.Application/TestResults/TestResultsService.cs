using Education.Application.Courses;
using Education.Application.Practicals;
using Education.Application.Users;
using Microsoft.Extensions.Logging;

namespace Education.Application.TestResults;

public sealed class TestResultsService(
    IEducationUserResolver userResolver,
    IPracticalsRepository practicalsRepository,
    ITestResultsRepository testResultsRepository,
    ILogger<TestResultsService> logger)
    : ITestResultsService
{
    public async Task<TestStatus> GetStatusAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveAssignedStudentAsync(practicalId, cancellationToken);
        return await testResultsRepository.GetStatusAsync(practicalId, userId, cancellationToken);
    }

    public async Task<int> StartTestAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveAssignedStudentAsync(practicalId, cancellationToken);
        var tryNumber = await testResultsRepository.StartTestAsync(practicalId, userId, cancellationToken);
        logger.LogInformation(
            "Студент {UserId} начал попытку {TryNumber} по практике {PracticalId}.",
            userId, tryNumber, practicalId);
        return tryNumber;
    }

    public async Task<TestQuestions?> GetQuestionsAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveAssignedStudentAsync(practicalId, cancellationToken);
        return await testResultsRepository.GetQuestionsAsync(practicalId, userId, cancellationToken);
    }

    public async Task<TestProtocolSummary> SubmitTestAsync(SubmitTestCommand command, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveAssignedStudentAsync(command.PracticalId, cancellationToken);
        var result = await testResultsRepository.SubmitTestAsync(command, userId, cancellationToken);
        logger.LogInformation(
            "Студент {UserId} сдал тест по практике {PracticalId}: попытка {TryNumber}, " +
            "балл {Score}/{MaxScore}, оценка {Grade}.",
            userId, command.PracticalId, result.TryNumber, result.Score, result.MaxScore, result.Grade);
        return result;
    }

    public async Task<IReadOnlyList<TestProtocolSummary>> GetStudentProtocolsAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveAssignedStudentAsync(practicalId, cancellationToken);
        return await testResultsRepository.GetStudentProtocolsAsync(practicalId, userId, cancellationToken);
    }

    public async Task<IReadOnlyList<TestProtocolSummary>> GetTeacherProtocolsAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);
        return await testResultsRepository.GetTeacherProtocolsAsync(practicalId, cancellationToken);
    }

    public Task<TestProtocol?> GetProtocolAsync(Guid testResultId, CancellationToken cancellationToken = default)
    {
        return testResultsRepository.GetProtocolAsync(testResultId, cancellationToken);
    }

    private async Task<Guid> ResolveAssignedStudentAsync(Guid practicalId, CancellationToken cancellationToken)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await testResultsRepository.IsPracticalAssignedToStudentAsync(practicalId, userId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в доступе к практике {PracticalId}: студент {UserId} не назначен.",
                practicalId, userId);
            throw new StudentPracticalAccessDeniedException(practicalId);
        }

        return userId;
    }

    private async Task EnsurePracticalOwnerAsync(Guid practicalId, CancellationToken cancellationToken)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await practicalsRepository.IsPracticalOwnerAsync(practicalId, userId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в доступе к протоколам практики {PracticalId}: пользователь {UserId} не владелец.",
                practicalId, userId);
            throw new CourseAccessDeniedException(practicalId);
        }
    }
}
