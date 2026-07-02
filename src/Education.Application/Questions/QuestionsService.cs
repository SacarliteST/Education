using Education.Application.Courses;
using Education.Application.Modules;
using Education.Application.Users;
using Education.Domain.Tests;

namespace Education.Application.Questions;

public sealed class QuestionsService(
    IEducationUserResolver userResolver,
    IModulesRepository modulesRepository,
    IQuestionsRepository questionsRepository)
    : IQuestionsService
{
    public Task<IReadOnlyList<Question>> GetQuestionsAsync(long moduleId, CancellationToken cancellationToken = default)
    {
        return questionsRepository.GetQuestionsAsync(moduleId, cancellationToken);
    }

    public async Task<Question> CreateQuestionAsync(CreateQuestionCommand command, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await modulesRepository.IsModuleOwnerAsync(command.ModuleId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(command.ModuleId);
        }

        return await questionsRepository.CreateQuestionAsync(command, cancellationToken);
    }

    public async Task UpdateQuestionAsync(long questionId, UpdateQuestionCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureQuestionOwnerAsync(questionId, cancellationToken);
        await questionsRepository.UpdateQuestionAsync(questionId, command, cancellationToken);
    }

    public async Task DeleteQuestionAsync(long questionId, CancellationToken cancellationToken = default)
    {
        await EnsureQuestionOwnerAsync(questionId, cancellationToken);
        await questionsRepository.DeleteQuestionAsync(questionId, cancellationToken);
    }

    private async Task EnsureQuestionOwnerAsync(long questionId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await questionsRepository.IsQuestionOwnerAsync(questionId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(questionId);
        }
    }
}
