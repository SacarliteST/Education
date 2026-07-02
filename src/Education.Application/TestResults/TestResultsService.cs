using Education.Application.Courses;
using Education.Application.Practicals;
using Education.Application.Users;

namespace Education.Application.TestResults;

public sealed class TestResultsService(
    IEducationUserResolver userResolver,
    IPracticalsRepository practicalsRepository,
    ITestResultsRepository testResultsRepository)
    : ITestResultsService
{
    public async Task<TestStatus> GetStatusAsync(long practicalId, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveAssignedStudentAsync(practicalId, cancellationToken);
        return await testResultsRepository.GetStatusAsync(practicalId, userId, cancellationToken);
    }

    public async Task<int> StartTestAsync(long practicalId, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveAssignedStudentAsync(practicalId, cancellationToken);
        return await testResultsRepository.StartTestAsync(practicalId, userId, cancellationToken);
    }

    public async Task<TestQuestions?> GetQuestionsAsync(long practicalId, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveAssignedStudentAsync(practicalId, cancellationToken);
        return await testResultsRepository.GetQuestionsAsync(practicalId, userId, cancellationToken);
    }

    public async Task<TestProtocolSummary> SubmitTestAsync(SubmitTestCommand command, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveAssignedStudentAsync(command.PracticalId, cancellationToken);
        return await testResultsRepository.SubmitTestAsync(command, userId, cancellationToken);
    }

    public async Task<IReadOnlyList<TestProtocolSummary>> GetStudentProtocolsAsync(long practicalId, CancellationToken cancellationToken = default)
    {
        var userId = await ResolveAssignedStudentAsync(practicalId, cancellationToken);
        return await testResultsRepository.GetStudentProtocolsAsync(practicalId, userId, cancellationToken);
    }

    public async Task<IReadOnlyList<TestProtocolSummary>> GetTeacherProtocolsAsync(long practicalId, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);
        return await testResultsRepository.GetTeacherProtocolsAsync(practicalId, cancellationToken);
    }

    public Task<TestProtocol?> GetProtocolAsync(long testResultId, CancellationToken cancellationToken = default)
    {
        return testResultsRepository.GetProtocolAsync(testResultId, cancellationToken);
    }

    private async Task<long> ResolveAssignedStudentAsync(long practicalId, CancellationToken cancellationToken)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await testResultsRepository.IsPracticalAssignedToStudentAsync(practicalId, userId, cancellationToken))
        {
            throw new StudentPracticalAccessDeniedException(practicalId);
        }

        return userId;
    }

    private async Task EnsurePracticalOwnerAsync(long practicalId, CancellationToken cancellationToken)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await practicalsRepository.IsPracticalOwnerAsync(practicalId, userId, cancellationToken))
        {
            throw new CourseAccessDeniedException(practicalId);
        }
    }
}
