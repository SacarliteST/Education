using Education.Application.Courses;
using Education.Application.Modules;
using Education.Application.Users;
using Education.Domain.Tests;
using Microsoft.Extensions.Logging;

namespace Education.Application.Questions;

/// <summary>Банк вопросов модуля: просмотр, создание, изменение и удаление.</summary>
public sealed class QuestionsService : IQuestionsService
{
    private readonly IEducationUserResolver userResolver;
    private readonly IModulesRepository modulesRepository;
    private readonly IQuestionsRepository questionsRepository;
    private readonly ILogger<QuestionsService> logger;

    /// <summary>Создаёт экземпляр класса <see cref="QuestionsService"/>.</summary>
    /// <param name="userResolver">Сопоставление пользователя Identity с учебным пользователем.</param>
    /// <param name="modulesRepository">Хранилище модулей курса.</param>
    /// <param name="questionsRepository">Хранилище вопросов.</param>
    /// <param name="logger">Журнал.</param>
    public QuestionsService(
        IEducationUserResolver userResolver,
        IModulesRepository modulesRepository,
        IQuestionsRepository questionsRepository,
        ILogger<QuestionsService> logger)
    {
        this.userResolver = userResolver;
        this.modulesRepository = modulesRepository;
        this.questionsRepository = questionsRepository;
        this.logger = logger;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Question>> GetQuestionsAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return questionsRepository.GetQuestionsAsync(moduleId, cancellationToken);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task UpdateQuestionAsync(Guid questionId, UpdateQuestionCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureQuestionOwnerAsync(questionId, cancellationToken);
        await questionsRepository.UpdateQuestionAsync(questionId, command, cancellationToken);
        logger.LogInformation("Вопрос {QuestionId} изменён (вес {Weight}).", questionId, command.Weight);
    }

    /// <inheritdoc />
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
