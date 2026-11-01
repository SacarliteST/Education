using Education.Application.Courses;
using Education.Application.Users;
using Microsoft.Extensions.Logging;

namespace Education.Application.AdminProfiles;

/// <summary>Администрирование учебных профилей и назначение студентов на курсы.</summary>
public sealed class AdminProfilesService : IAdminProfilesService
{
    private readonly IAdminProfilesRepository repository;
    private readonly IEducationUserResolver userResolver;
    private readonly ICoursesRepository coursesRepository;
    private readonly ILogger<AdminProfilesService> logger;

    /// <summary>Создаёт экземпляр класса <see cref="AdminProfilesService"/>.</summary>
    /// <param name="repository">Хранилище учебных профилей.</param>
    /// <param name="userResolver">Сопоставление пользователя Identity с учебным пользователем.</param>
    /// <param name="coursesRepository">Хранилище курсов.</param>
    /// <param name="logger">Журнал.</param>
    public AdminProfilesService(
        IAdminProfilesRepository repository,
        IEducationUserResolver userResolver,
        ICoursesRepository coursesRepository,
        ILogger<AdminProfilesService> logger)
    {
        this.repository = repository;
        this.userResolver = userResolver;
        this.coursesRepository = coursesRepository;
        this.logger = logger;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<AdminProfile>> GetProfilesAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetProfilesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AdminProfile> CreateLinkedProfileAsync(
        CreateAdminProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var profile = await repository.CreateLinkedProfileAsync(command, cancellationToken);
        logger.LogInformation(
            "Создан/восстановлен профиль «{Login}» (роль {RoleId}), идентификатор {UserId}.",
            command.Login, command.RoleId, command.UserId);
        return profile;
    }

    /// <inheritdoc />
    public Task<AdminProfile?> UpdateProfileAsync(
        Guid legacyUserId,
        UpdateAdminProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        return repository.UpdateProfileAsync(legacyUserId, command, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeactivateProfileLinkAsync(Guid legacyUserId, CancellationToken cancellationToken = default)
    {
        var deactivated = await repository.DeactivateProfileLinkAsync(legacyUserId, cancellationToken);
        if (deactivated)
        {
            logger.LogInformation("Отвязан профиль {LegacyUserId}.", legacyUserId);
        }

        return deactivated;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AssignableStudent>> GetAssignableStudentsForCourseAsync(
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        await EnsureCourseOwnerAsync(courseId, cancellationToken);
        return await repository.GetAssignableStudentsForCourseAsync(courseId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetCourseStudentsAsync(
        SetCourseStudentsCommand command,
        CancellationToken cancellationToken = default)
    {
        await EnsureCourseOwnerAsync(command.CourseId, cancellationToken);
        await repository.SetCourseStudentsAsync(command, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AssignableStudentsPage> GetAssignableStudentsPageForCourseAsync(
        Guid courseId,
        AssignableStudentsQuery query,
        CancellationToken cancellationToken = default)
    {
        await EnsureCourseOwnerAsync(courseId, cancellationToken);
        return await repository.GetAssignableStudentsPageForCourseAsync(courseId, query, cancellationToken);
    }

    /// <inheritdoc />
    public async Task ChangeCourseStudentsAsync(
        ChangeCourseStudentsCommand command,
        CancellationToken cancellationToken = default)
    {
        await EnsureCourseOwnerAsync(command.CourseId, cancellationToken);
        await repository.ChangeCourseStudentsAsync(command, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<StudentGroup>> GetStudentGroupsAsync(CancellationToken cancellationToken = default)
    {
        return repository.GetStudentGroupsAsync(cancellationToken);
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
}
