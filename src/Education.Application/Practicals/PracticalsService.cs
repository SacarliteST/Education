using Education.Application.Courses;
using Education.Application.Modules;
using Education.Application.PracticalModules;
using Education.Application.Questions;
using Education.Application.Users;
using Education.Domain.Practicals;
using Microsoft.Extensions.Logging;

namespace Education.Application.Practicals;

public sealed class PracticalsService(
    IEducationUserResolver userResolver,
    IModulesRepository modulesRepository,
    IPracticalsRepository practicalsRepository,
    IPracticalModulesRepository practicalModulesRepository,
    IQuestionsRepository questionsRepository,
    ILogger<PracticalsService> logger)
    : IPracticalsService
{
    public Task<IReadOnlyList<PracticalMaterial>> GetPracticalsAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return practicalsRepository.GetPracticalsAsync(moduleId, cancellationToken);
    }

    public Task<PracticalDetail?> GetDetailAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        return practicalsRepository.GetDetailAsync(practicalId, cancellationToken);
    }

    public async Task<PracticalMaterial> CreatePracticalAsync(CreatePracticalCommand command, CancellationToken cancellationToken = default)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await modulesRepository.IsModuleOwnerAsync(command.ModuleId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в создании практики в модуле {ModuleId}: пользователь {LegacyUserId} не владелец.",
                command.ModuleId, legacyUserId);
            throw new CourseAccessDeniedException(command.ModuleId);
        }

        var practical = await practicalsRepository.CreatePracticalAsync(command, cancellationToken);
        logger.LogInformation(
            "Практика {PracticalId} создана в модуле {ModuleId} преподавателем {LegacyUserId}.",
            practical.Id, command.ModuleId, legacyUserId);
        return practical;
    }

    public async Task PublishPracticalAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);
        await practicalsRepository.PublishPracticalAsync(practicalId, cancellationToken);
        logger.LogInformation("Практика {PracticalId} опубликована.", practicalId);
    }

    public async Task DeletePracticalAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);
        await practicalsRepository.DeletePracticalAsync(practicalId, cancellationToken);
        logger.LogInformation("Практика {PracticalId} удалена.", practicalId);
    }

    public Task<IReadOnlyList<Case>> GetTasksAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        return practicalsRepository.GetTasksAsync(practicalId, cancellationToken);
    }

    public Task<Case?> GetTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return practicalsRepository.GetTaskAsync(taskId, cancellationToken);
    }

    public async Task<Case> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(command.PracticalId, cancellationToken);
        var task = await practicalsRepository.CreateTaskAsync(command, cancellationToken);
        logger.LogInformation("Задание {TaskId} создано в практике {PracticalId}.", task.Id, command.PracticalId);
        return task;
    }

    public async Task UpdateTaskTextAsync(Guid taskId, UpdateTaskTextCommand command, CancellationToken cancellationToken = default)
    {
        await EnsureTaskOwnerAsync(taskId, cancellationToken);
        await practicalsRepository.UpdateTaskTextAsync(taskId, command.Text, cancellationToken);
    }

    public async Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        await EnsureTaskOwnerAsync(taskId, cancellationToken);
        await practicalsRepository.DeleteTaskAsync(taskId, cancellationToken);
        logger.LogInformation("Задание {TaskId} удалено.", taskId);
    }

    public async Task<PracticalQuestionsSetup?> GetQuestionsSetupAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);
        return await practicalsRepository.GetQuestionsSetupAsync(practicalId, cancellationToken);
    }

    private const double MaxQuestionsWeightSum = 100;

    public async Task ConfigureQuestionsAsync(ConfigurePracticalQuestionsCommand command, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(command.PracticalId, cancellationToken);

        var totalWeight = await questionsRepository.SumWeightsAsync(
            command.QuestionIds.ToArray(), cancellationToken);
        if (totalWeight > MaxQuestionsWeightSum)
        {
            logger.LogWarning(
                "Отклонена настройка вопросов практики {PracticalId}: сумма весов {TotalWeight} превышает {Max}.",
                command.PracticalId, totalWeight, MaxQuestionsWeightSum);
            throw new PracticalQuestionsWeightExceededException(totalWeight);
        }

        await practicalsRepository.ConfigureQuestionsAsync(command, cancellationToken);
        logger.LogInformation(
            "Настроены вопросы практики {PracticalId}: {QuestionCount} вопросов, сумма весов {TotalWeight}.",
            command.PracticalId, command.QuestionIds.Count, totalWeight);
    }

    public async Task BindModuleAsync(BindPracticalModuleCommand command, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(command.PracticalId, cancellationToken);

        var module = await practicalModulesRepository.GetByIdAsync(command.PracticalModuleId, cancellationToken);
        if (module is null || !module.IsEnabled)
        {
            logger.LogWarning(
                "Отклонена привязка практики {PracticalId} к модулю {PracticalModuleId}: модуль не найден или отключён.",
                command.PracticalId, command.PracticalModuleId);
            throw new PracticalModuleNotFoundException(command.PracticalModuleId);
        }

        var practical = await practicalsRepository.GetByIdAsync(command.PracticalId, cancellationToken);
        // Первая привязка не должна затирать файловую практику с работой студентов.
        // Перепривязка уже внешней практики безопасна — там только один служебный Case.
        if (practical is { Kind: not PracticalKind.External }
            && await practicalsRepository.HasStudentActivityAsync(command.PracticalId, cancellationToken))
        {
            logger.LogWarning(
                "Отклонена привязка практики {PracticalId} к модулю {PracticalModuleId}: уже есть активность студентов.",
                command.PracticalId, command.PracticalModuleId);
            throw new PracticalHasActivityException();
        }

        await practicalsRepository.BindModuleAsync(command, cancellationToken);
        logger.LogInformation(
            "Практика {PracticalId} привязана к модулю {PracticalModuleId} (задание {ExternalTaskRef}).",
            command.PracticalId, command.PracticalModuleId, command.ExternalTaskRef);
    }

    private async Task EnsurePracticalOwnerAsync(Guid practicalId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await practicalsRepository.IsPracticalOwnerAsync(practicalId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в доступе к практике {PracticalId}: пользователь {LegacyUserId} не владелец.",
                practicalId, legacyUserId);
            throw new CourseAccessDeniedException(practicalId);
        }
    }

    private async Task EnsureTaskOwnerAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await practicalsRepository.IsTaskOwnerAsync(taskId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в доступе к заданию {TaskId}: пользователь {LegacyUserId} не владелец.",
                taskId, legacyUserId);
            throw new CourseAccessDeniedException(taskId);
        }
    }
}
