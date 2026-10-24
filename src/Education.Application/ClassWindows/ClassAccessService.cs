using Education.Application.Users;
using Microsoft.Extensions.Logging;

namespace Education.Application.ClassWindows;

public sealed class ClassAccessService(
    IClassWindowsRepository repository,
    IUserProfileRepository profiles,
    TimeProvider timeProvider,
    ILogger<ClassAccessService> logger) : IClassAccessService
{
    /// <summary>Сколько после конца пары ещё принимается сдача уже начатого теста.</summary>
    public static readonly TimeSpan SubmitGrace = TimeSpan.FromSeconds(60);

    public async Task<DateTimeOffset?> RequireWindowEndAsync(
        Guid courseId, Guid userId, CancellationToken cancellationToken = default)
    {
        var state = await GetStateAsync(courseId, userId, cancellationToken);
        if (!state.PracticeOnlyDuringClass)
        {
            return null;
        }

        if (!state.IsOpen)
        {
            Deny(courseId, userId);
        }

        return state.EndsAt;
    }

    public async Task EnsureOpenForPracticalAsync(
        Guid practicalId, Guid userId, bool withGrace = false, CancellationToken cancellationToken = default)
    {
        var gate = await repository.GetGateForPracticalAsync(practicalId, cancellationToken);
        await EnsureAsync(gate, userId, withGrace ? SubmitGrace : TimeSpan.Zero, cancellationToken);
    }

    public async Task EnsureOpenForTaskAsync(Guid taskId, Guid userId, CancellationToken cancellationToken = default)
    {
        var gate = await repository.GetGateForTaskAsync(taskId, cancellationToken);
        await EnsureAsync(gate, userId, TimeSpan.Zero, cancellationToken);
    }

    public async Task<ClassAccessState> GetStateAsync(
        Guid courseId, Guid userId, CancellationToken cancellationToken = default)
    {
        var course = await repository.GetCourseAsync(courseId, cancellationToken);
        if (course is null || !course.PracticeOnlyDuringClass)
        {
            return new ClassAccessState(false, true, null);
        }

        var window = await FindWindowAsync(courseId, userId, TimeSpan.Zero, cancellationToken);
        return new ClassAccessState(true, window is not null, window?.EffectiveEnd);
    }

    private async Task EnsureAsync(
        PracticeGate? gate, Guid userId, TimeSpan grace, CancellationToken cancellationToken)
    {
        if (gate is not { PracticeOnlyDuringClass: true })
        {
            return;
        }

        var window = await FindWindowAsync(gate.CourseId, userId, grace, cancellationToken);
        if (window is null)
        {
            Deny(gate.CourseId, userId);
        }
    }

    private async Task<Education.Domain.Courses.ClassWindow?> FindWindowAsync(
        Guid courseId, Guid userId, TimeSpan grace, CancellationToken cancellationToken)
    {
        var profile = await profiles.FindProfileAsync(userId, cancellationToken);
        var group = profile?.GroupName?.Trim();
        if (String.IsNullOrEmpty(group))
        {
            return null;
        }

        return await repository.FindOpenAsync(courseId, group, timeProvider.GetUtcNow(), grace, cancellationToken);
    }

    private void Deny(Guid courseId, Guid userId)
    {
        logger.LogWarning(
            "Отказано в доступе к практике курса {CourseId}: у группы пользователя {UserId} нет идущей пары.",
            courseId, userId);
        throw new ClassWindowClosedException();
    }
}
