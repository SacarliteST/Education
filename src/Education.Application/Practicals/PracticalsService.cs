using Education.Application.Courses;
using Education.Application.Modules;
using Education.Application.Users;
using Education.Domain.Practicals;

namespace Education.Application.Practicals;

public sealed class PracticalsService(
    IEducationUserResolver userResolver,
    IModulesRepository modulesRepository,
    IPracticalsRepository practicalsRepository)
    : IPracticalsService
{
    public Task<IReadOnlyList<PracticalMaterial>> GetPracticalsAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return practicalsRepository.GetPracticalsAsync(moduleId, cancellationToken);
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

    public Task<IReadOnlyList<Case>> GetTasksAsync(Guid practicalId, CancellationToken cancellationToken = default)
    {
        return practicalsRepository.GetTasksAsync(practicalId, cancellationToken);
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

    private async Task EnsurePracticalOwnerAsync(Guid practicalId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await practicalsRepository.IsPracticalOwnerAsync(practicalId, legacyUserId, cancellationToken))
        {
            throw new CourseAccessDeniedException(practicalId);
        }
    }
}

