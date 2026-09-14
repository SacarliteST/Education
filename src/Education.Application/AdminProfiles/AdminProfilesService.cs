using Education.Application.Audit;
using Education.Application.Courses;
using Education.Application.Practicals;
using Education.Application.Users;
using Microsoft.Extensions.Logging;

namespace Education.Application.AdminProfiles;

public sealed class AdminProfilesService(
    IAdminProfilesRepository repository,
    IEducationUserResolver userResolver,
    ICoursesRepository coursesRepository,
    IPracticalsRepository practicalsRepository,
    IAdminEventRecorder eventRecorder,
    ILogger<AdminProfilesService> logger) : IAdminProfilesService
{
    public Task<IReadOnlyList<AdminProfile>> GetProfilesAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetProfilesAsync(cancellationToken);
    }

    public async Task<AdminProfile> CreateLinkedProfileAsync(
        CreateAdminProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var profile = await repository.CreateLinkedProfileAsync(command, cancellationToken);
        await eventRecorder.RecordAsync(
            AdminEventTypes.ProfileLinked,
            $"Связан профиль «{command.Login}» (роль {command.RoleId}) с identity-пользователем {command.IdentityUserId}.",
            cancellationToken);
        return profile;
    }

    public Task<AdminProfile?> UpdateProfileAsync(
        Guid legacyUserId,
        UpdateAdminProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        return repository.UpdateProfileAsync(legacyUserId, command, cancellationToken);
    }

    public async Task<bool> DeactivateProfileLinkAsync(Guid legacyUserId, CancellationToken cancellationToken = default)
    {
        var deactivated = await repository.DeactivateProfileLinkAsync(legacyUserId, cancellationToken);
        if (deactivated)
        {
            await eventRecorder.RecordAsync(
                AdminEventTypes.ProfileUnlinked,
                $"Отвязан профиль {legacyUserId}.",
                cancellationToken);
        }

        return deactivated;
    }

    public async Task<IReadOnlyList<AssignableStudent>> GetAssignableStudentsForCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        await EnsureCourseOwnerAsync(courseId, cancellationToken);
        return await repository.GetAssignableStudentsForCourseAsync(courseId, cancellationToken);
    }

    public async Task<IReadOnlyList<AssignableStudent>> GetAssignableStudentsForPracticalAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(practicalId, cancellationToken);
        return await repository.GetAssignableStudentsForPracticalAsync(practicalId, cancellationToken);
    }

    public async Task SetCourseStudentsAsync(
        SetCourseStudentsCommand command,
        CancellationToken cancellationToken = default)
    {
        await EnsureCourseOwnerAsync(command.CourseId, cancellationToken);
        await repository.SetCourseStudentsAsync(command, cancellationToken);
    }

    public async Task SetPracticalStudentsAsync(
        SetPracticalStudentsCommand command,
        CancellationToken cancellationToken = default)
    {
        await EnsurePracticalOwnerAsync(command.PracticalId, cancellationToken);
        await repository.SetPracticalStudentsAsync(command, cancellationToken);
    }

    private async Task EnsureCourseOwnerAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsCourseOwnerAsync(courseId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в назначении студентов на курс {CourseId}: пользователь {LegacyUserId} не владелец.",
                courseId, legacyUserId);
            throw new CourseAccessDeniedException(courseId);
        }
    }

    private async Task EnsurePracticalOwnerAsync(Guid practicalId, CancellationToken cancellationToken)
    {
        var legacyUserId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await practicalsRepository.IsPracticalOwnerAsync(practicalId, legacyUserId, cancellationToken))
        {
            logger.LogWarning(
                "Отказано в назначении студентов на практику {PracticalId}: пользователь {LegacyUserId} не владелец.",
                practicalId, legacyUserId);
            throw new CourseAccessDeniedException(practicalId);
        }
    }
}
