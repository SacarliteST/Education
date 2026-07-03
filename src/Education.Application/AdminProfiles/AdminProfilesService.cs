namespace Education.Application.AdminProfiles;

public sealed class AdminProfilesService(IAdminProfilesRepository repository) : IAdminProfilesService
{
    public Task<IReadOnlyList<AdminProfile>> GetProfilesAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetProfilesAsync(cancellationToken);
    }

    public Task<AdminProfile> CreateLinkedProfileAsync(
        CreateAdminProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        return repository.CreateLinkedProfileAsync(command, cancellationToken);
    }

    public Task<AdminProfile?> UpdateProfileAsync(
        Guid legacyUserId,
        UpdateAdminProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        return repository.UpdateProfileAsync(legacyUserId, command, cancellationToken);
    }

    public Task<bool> DeactivateProfileLinkAsync(Guid legacyUserId, CancellationToken cancellationToken = default)
    {
        return repository.DeactivateProfileLinkAsync(legacyUserId, cancellationToken);
    }

    public Task<IReadOnlyList<AssignableStudent>> GetAssignableStudentsForCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetAssignableStudentsForCourseAsync(courseId, cancellationToken);
    }

    public Task<IReadOnlyList<AssignableStudent>> GetAssignableStudentsForPracticalAsync(
        Guid practicalId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetAssignableStudentsForPracticalAsync(practicalId, cancellationToken);
    }
}

