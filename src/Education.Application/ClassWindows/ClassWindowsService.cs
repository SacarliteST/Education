using Education.Application.Courses;
using Education.Application.PracticalModules;
using Education.Application.Users;
using Education.Domain.Courses;
using Education.Domain.PracticalModules;
using Microsoft.Extensions.Logging;

namespace Education.Application.ClassWindows;

/// <summary>Управление парами курса: запуск, продление, завершение и режим «практика только на парах».</summary>
public sealed class ClassWindowsService : IClassWindowsService
{
    private readonly IEducationUserResolver userResolver;
    private readonly ICoursesRepository coursesRepository;
    private readonly IClassWindowsRepository repository;
    private readonly IPracticalModulesRepository modulesRepository;
    private readonly IPracticalModuleSessionsRepository sessionsRepository;
    private readonly IModulePushClient pushClient;
    private readonly TimeProvider timeProvider;
    private readonly ILogger<ClassWindowsService> logger;

    /// <summary>Создаёт экземпляр класса <see cref="ClassWindowsService"/>.</summary>
    /// <param name="userResolver">Сопоставление пользователя Identity с учебным пользователем.</param>
    /// <param name="coursesRepository">Хранилище курсов.</param>
    /// <param name="repository">Хранилище пар.</param>
    /// <param name="modulesRepository">Реестр внешних практических модулей.</param>
    /// <param name="sessionsRepository">Хранилище сессий внешних модулей.</param>
    /// <param name="pushClient">Клиент передачи сессии бэкенду модуля.</param>
    /// <param name="timeProvider">Источник текущего времени.</param>
    /// <param name="logger">Журнал.</param>
    public ClassWindowsService(
        IEducationUserResolver userResolver,
        ICoursesRepository coursesRepository,
        IClassWindowsRepository repository,
        IPracticalModulesRepository modulesRepository,
        IPracticalModuleSessionsRepository sessionsRepository,
        IModulePushClient pushClient,
        TimeProvider timeProvider,
        ILogger<ClassWindowsService> logger)
    {
        this.userResolver = userResolver;
        this.coursesRepository = coursesRepository;
        this.repository = repository;
        this.modulesRepository = modulesRepository;
        this.sessionsRepository = sessionsRepository;
        this.pushClient = pushClient;
        this.timeProvider = timeProvider;
        this.logger = logger;
    }

    private const int HistorySize = 30;

    /// <inheritdoc />
    public async Task<CourseClassWindows> GetAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        await EnsureCourseOwnerAsync(courseId, cancellationToken);
        var course = await repository.GetCourseAsync(courseId, cancellationToken)
            ?? throw new CourseAccessDeniedException(courseId);
        var groups = await repository.ListGroupsAsync(courseId, cancellationToken);
        var windows = await repository.ListForCourseAsync(courseId, HistorySize, cancellationToken);
        var now = timeProvider.GetUtcNow();
        return new CourseClassWindows(
            course.PracticeOnlyDuringClass,
            groups,
            windows.Select(window => ToInfo(window, now)).ToList());
    }

    /// <inheritdoc />
    public async Task SetModeAsync(Guid courseId, bool practiceOnlyDuringClass, CancellationToken cancellationToken = default)
    {
        await EnsureCourseOwnerAsync(courseId, cancellationToken);
        var course = await repository.GetCourseAsync(courseId, cancellationToken)
            ?? throw new CourseAccessDeniedException(courseId);
        course.SetPracticeOnlyDuringClass(practiceOnlyDuringClass);
        await repository.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Режим «практика только на парах» курса {CourseId}: {Value}.", courseId, practiceOnlyDuringClass);
    }

    /// <inheritdoc />
    public async Task<ClassWindowInfo> StartAsync(
        Guid courseId, string groupName, int durationMinutes, CancellationToken cancellationToken = default)
    {
        var teacherId = await EnsureCourseOwnerAsync(courseId, cancellationToken);
        var group = groupName?.Trim();
        if (String.IsNullOrEmpty(group))
        {
            throw new ClassWindowGroupRequiredException();
        }

        var now = timeProvider.GetUtcNow();
        var existing = await repository.GetUnclosedAsync(courseId, group, cancellationToken);
        if (existing is not null)
        {
            if (existing.IsOpenAt(now))
            {
                throw new ClassWindowAlreadyOpenException();
            }

            // Просроченная, но не закрытая вручную пара мешает уникальному индексу — закрываем.
            existing.Finish(existing.EndsAt, teacherId);
            await repository.SaveChangesAsync(cancellationToken);
        }

        var window = ClassWindow.Start(courseId, group, now, durationMinutes, teacherId);
        await repository.AddAsync(window, cancellationToken);
        logger.LogInformation(
            "Начата пара {WindowId}: курс {CourseId}, группа {Group}, {Minutes} мин, преподаватель {TeacherId}.",
            window.Id, courseId, group, durationMinutes, teacherId);
        return ToInfo(window, now);
    }

    /// <inheritdoc />
    public async Task<ClassWindowInfo> ExtendAsync(Guid windowId, int minutes, CancellationToken cancellationToken = default)
    {
        var window = await repository.GetByIdAsync(windowId, cancellationToken)
            ?? throw new ClassWindowNotFoundException();
        await EnsureCourseOwnerAsync(window.CourseId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        if (!window.IsOpenAt(now))
        {
            throw new ClassWindowAlreadyFinishedException();
        }

        window.Extend(minutes, now);
        await repository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Пара {WindowId} продлена на {Minutes} мин, конец {EndsAt}.", windowId, minutes, window.EndsAt);
        await ApplyExpiryAsync(window, now, finished: false, cancellationToken);
        return ToInfo(window, now);
    }

    /// <inheritdoc />
    public async Task<ClassWindowInfo> FinishAsync(Guid windowId, CancellationToken cancellationToken = default)
    {
        var window = await repository.GetByIdAsync(windowId, cancellationToken)
            ?? throw new ClassWindowNotFoundException();
        var teacherId = await EnsureCourseOwnerAsync(window.CourseId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        if (!window.IsOpenAt(now))
        {
            throw new ClassWindowAlreadyFinishedException();
        }

        window.Finish(now, teacherId);
        await repository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Пара {WindowId} завершена досрочно преподавателем {TeacherId}.", windowId, teacherId);
        await ApplyExpiryAsync(window, now, finished: true, cancellationToken);
        return ToInfo(window, now);
    }

    /// <summary>
    /// Переносит новый срок пары на идущие сессии группы: у модуля обновляется <c>expiresAt</c>, у платформы — срок сессии.
    /// Сбой пуша не откатывает пару — только пишется в лог (сессия закроется по прежнему сроку).
    /// </summary>
    private async Task ApplyExpiryAsync(
        ClassWindow window, DateTimeOffset now, bool finished, CancellationToken cancellationToken)
    {
        var targets = await repository.ListActiveSessionsAsync(window.CourseId, window.GroupName, cancellationToken);
        foreach (var target in targets)
        {
            var session = target.Session;
            var expiresAt = window.EffectiveEnd;
            if (target.TimeLimitMinutes is { } limit)
            {
                var byLimit = session.StartedAt.AddMinutes(limit);
                if (byLimit < expiresAt)
                {
                    expiresAt = byLimit;
                }
            }

            if (session.ExpiresAt == expiresAt)
            {
                continue;
            }

            var module = await modulesRepository.GetByIdAsync(target.PracticalModuleId, cancellationToken);
            if (module is null)
            {
                continue;
            }

            try
            {
                session.ChangeExpiry(expiresAt);
                await pushClient.PushAsync(
                    ModuleSessionsService.ReadSessionsEndpoint(module),
                    module.Slug,
                    new ModuleSessionPush(
                        session.Id,
                        session.SessionKey,
                        session.UserId,
                        target.ExternalTaskRef,
                        session.ReturnUrl,
                        expiresAt,
                        target.AttemptsPerSession),
                    cancellationToken);
            }
            catch (Exception exception) when (exception is ModulePushFailedException or HttpRequestException)
            {
                logger.LogWarning(
                    exception,
                    "Не удалось передать модулю новый срок сессии {SessionId} (пара {WindowId}); модуль закроет её по прежнему сроку.",
                    session.Id, window.Id);
            }
        }

        await sessionsRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<Guid> EnsureCourseOwnerAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var userId = await userResolver.ResolveCurrentLegacyUserIdAsync(cancellationToken);
        if (!await coursesRepository.IsCourseOwnerAsync(courseId, userId, cancellationToken))
        {
            logger.LogWarning("Отказано в управлении парами курса {CourseId}: пользователь {UserId} не владелец.", courseId, userId);
            throw new CourseAccessDeniedException(courseId);
        }

        return userId;
    }

    private static ClassWindowInfo ToInfo(ClassWindow window, DateTimeOffset now) => new(
        window.Id,
        window.CourseId,
        window.GroupName,
        window.StartsAt,
        window.EffectiveEnd,
        window.ClosedAt,
        window.IsOpenAt(now));
}
