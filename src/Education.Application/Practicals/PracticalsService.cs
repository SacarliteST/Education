using Education.Application.Courses;
using Education.Application.Modules;
using Education.Application.PracticalModules;
using Education.Application.Users;
using Education.Domain.Practicals;

namespace Education.Application.Practicals;

public sealed class PracticalsService(
    IEducationUserResolver userResolver,
    IModulesRepository modulesRepository,
    IPracticalsRepository practicalsRepository,
    IPracticalModulesRepository practicalModulesRepository)
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
            throw new CourseAccessDeniedException(command.ModuleId);
        }

        return await practicalsRepository.CreatePracticalAsync(command, cancellationToken);
    }

    public async Task PublishPracticalAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);
        await practicalsRepository.PublishPracticalAsync(practicalId, cancellationToken);
    }

    public async Task DeletePracticalAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);
        await practicalsRepository.DeletePracticalAsync(practicalId, cancellationToken);
    }

    public Task<IReadOnlyList<Case>> GetTasksAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        return practicalsRepository.GetTasksAsync(practicalId, cancellationToken);
    }

    public async Task<Case> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(command.PracticalId, cancellationToken);
        return await practicalsRepository.CreateTaskAsync(command, cancellationToken);
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
    }

    public async Task<PracticalQuestionsSetup?> GetQuestionsSetupAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);
        return await practicalsRepository.GetQuestionsSetupAsync(practicalId, cancellationToken);
    }

    public async Task ConfigureQuestionsAsync(ConfigurePracticalQuestionsCommand command, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(command.PracticalId, cancellationToken);
        await practicalsRepository.ConfigureQuestionsAsync(command, cancellationToken);
    }

    public async Task BindModuleAsync(BindPracticalModuleCommand command, CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(command.PracticalId, cancellationToken);

        var module = await practicalModulesRepository.GetByIdAsync(command.PracticalModuleId, cancellationToken);
        if (module is null || !module.IsEnabled)
        {
            throw new PracticalModuleNotFoundException(command.PracticalModuleId);
        }

        var practical = await practicalsRepository.GetByIdAsync(command.PracticalId, cancellationToken);
        // Первая привязка не должна затирать файловую практику с работой студентов.
        // Перепривязка уже внешней практики безопасна — там только один служебный Case.
        if (practical is { Kind: not PracticalKind.External }
            && await practicalsRepository.HasStudentActivityAsync(command.PracticalId, cancellationToken))
        {
            throw new PracticalHasActivityException();
        }

        await practicalsRepository.BindModuleAsync(command, cancellationToken);
    }

    private async Task EnsurePracticalOwnerAsync(Guid practicalId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await practicalsRepository.IsPracticalOwnerAsync(practicalId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(practicalId);
        }
    }

    private async Task EnsureTaskOwnerAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await practicalsRepository.IsTaskOwnerAsync(taskId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(taskId);
        }
    }
}

