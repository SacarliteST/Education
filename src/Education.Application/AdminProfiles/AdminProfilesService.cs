using Education.Application.Audit;
using Education.Application.Courses;
using Education.Application.Practicals;
using Education.Application.Users;

namespace Education.Application.AdminProfiles;

public sealed class AdminProfilesService(
    IAdminProfilesRepository repository,
    IEducationUserResolver userResolver,
    ICoursesRepository coursesRepository,
    IPracticalsRepository practicalsRepository,
    IAdminEventRecorder eventRecorder) : IAdminProfilesService
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
            throw new CourseAccessDeniedException(courseId);
        }
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
