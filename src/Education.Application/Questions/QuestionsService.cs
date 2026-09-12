using Education.Application.Courses;
using Education.Application.Modules;
using Education.Application.Users;
using Education.Domain.Tests;
using Microsoft.Extensions.Logging;

namespace Education.Application.Questions;

public sealed class QuestionsService(
    IEducationUserResolver userResolver,
    IModulesRepository modulesRepository,
    IQuestionsRepository questionsRepository,
    ILogger<QuestionsService> logger)
    : IQuestionsService
{
    public Task<IReadOnlyList<Question>> GetQuestionsAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return questionsRepository.GetQuestionsAsync(moduleId, cancellationToken);
    }

    public async Task<Question> CreateQuestionAsync(CreateQuestionCommand command, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await modulesRepository.IsModuleOwnerAsync(command.ModuleId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в создании вопроса в модуле {ModuleId}: пользователь {LegacyUserId} не владелец.",
                command.ModuleId, legacyUserId);
            throw new CourseAccessDeniedException(command.ModuleId);
        }

        var question = await questionsRepository.CreateQuestionAsync(command, cancellationToken);
        logger.LogInformation(
            "Вопрос {QuestionId} создан в модуле {ModuleId} (вес {Weight}).",
            question.Id, command.ModuleId, command.Weight);
        return question;
    }

    public async Task UpdateQuestionAsync(Guid questionId, UpdateQuestionCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureQuestionOwnerAsync(questionId, cancellationToken);
        await questionsRepository.UpdateQuestionAsync(questionId, command, cancellationToken);
        logger.LogInformation("Вопрос {QuestionId} изменён (вес {Weight}).", questionId, command.Weight);
    }

    public async Task DeleteQuestionAsync(Guid questionId, CancellationToken cancellationToken = default)
    {
        await EnsureQuestionOwnerAsync(questionId, cancellationToken);
        await questionsRepository.DeleteQuestionAsync(questionId, cancellationToken);
        logger.LogInformation("Вопрос {QuestionId} удалён.", questionId);
    }

    private async Task EnsureQuestionOwnerAsync(Guid questionId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await questionsRepository.IsQuestionOwnerAsync(questionId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в доступе к вопросу {QuestionId}: пользователь {LegacyUserId} не владелец.",
                questionId, legacyUserId);
            throw new CourseAccessDeniedException(questionId);
        }
    }
}
