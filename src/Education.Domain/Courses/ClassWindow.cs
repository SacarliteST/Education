using Education.Domain.Common;

namespace Education.Domain.Courses;

/// <summary>
/// «Пара» — окно доступа к тестам и практике курса: начинает преподаватель, задаёт длительность.
/// Доступ открывается группе (<see cref="GroupName"/>) и/или перечисленным студентам (<see cref="Students"/>).
/// </summary>
public sealed class ClassWindow : Entity
{
    /// <summary>Минимальная и максимальная длительность пары при запуске, минут.</summary>
    public const int MinDurationMinutes = 5;

    /// <summary>Максимальная длительность пары при запуске, минут.</summary>
    public const int MaxDurationMinutes = 480;

    /// <summary>Максимальная добавка при продлении, минут.</summary>
    public const int MaxExtendMinutes = 240;

    /// <summary>Курс.</summary>
    public Guid CourseId { get; private set; }

    /// <summary>Группа студентов (как в <c>User.GroupName</c>); <see langword="null"/> — пара только для перечисленных студентов.</summary>
    public string? GroupName { get; private set; }

    /// <summary>Студенты с точечным доступом к паре.</summary>
    public List<ClassWindowStudent> Students { get; private set; } = [];

    /// <summary>Начало пары (UTC).</summary>
    public DateTimeOffset StartsAt { get; private set; }

    /// <summary>Плановый конец пары (UTC); продление сдвигает его.</summary>
    public DateTimeOffset EndsAt { get; private set; }

    /// <summary>Кто начал пару.</summary>
    public Guid StartedByUserId { get; private set; }

    /// <summary>Когда пару завершили вручную; <see langword="null"/> — не завершена вручную.</summary>
    public DateTimeOffset? ClosedAt { get; private set; }

    /// <summary>Кто завершил пару вручную.</summary>
    public Guid? ClosedByUserId { get; private set; }

    /// <summary>Момент создания записи (UTC).</summary>
    public DateTimeOffset CreatedAt { get; private set; }

    private ClassWindow()
    {
    }

    /// <summary>Начинает пару на заданную длительность. Нужна группа и/или хотя бы один студент.</summary>
    public static ClassWindow Start(
        Guid courseId, string? groupName, IReadOnlyCollection<Guid> studentUserIds,
        DateTimeOffset now, int durationMinutes, Guid startedByUserId)
    {
        if (durationMinutes is < MinDurationMinutes or > MaxDurationMinutes)
        {
            throw new ArgumentOutOfRangeException(nameof(durationMinutes));
        }

        var group = groupName?.Trim();
        if (String.IsNullOrEmpty(group) && studentUserIds.Count == 0)
        {
            throw new ArgumentException("Нужна группа или хотя бы один студент.", nameof(studentUserIds));
        }

        var start = now.ToUniversalTime();
        var window = new ClassWindow
        {
            CourseId = courseId,
            GroupName = String.IsNullOrEmpty(group) ? null : group,
            StartsAt = start,
            EndsAt = start.AddMinutes(durationMinutes),
            StartedByUserId = startedByUserId,
            CreatedAt = start,
        };
        foreach (var userId in studentUserIds.Distinct())
        {
            window.Students.Add(ClassWindowStudent.Create(userId));
        }

        return window;
    }

    /// <summary>Реальный конец пары: ручное завершение, если оно раньше планового конца.</summary>
    public DateTimeOffset EffectiveEnd => ClosedAt is { } closed && closed < EndsAt ? closed : EndsAt;

    /// <summary>Пара идёт в момент <paramref name="now"/>.</summary>
    public bool IsOpenAt(DateTimeOffset now) => StartsAt <= now && now < EffectiveEnd;

    /// <summary>Продлевает идущую пару.</summary>
    public void Extend(int minutes, DateTimeOffset now)
    {
        if (minutes is < 1 or > MaxExtendMinutes)
        {
            throw new ArgumentOutOfRangeException(nameof(minutes));
        }

        if (!IsOpenAt(now))
        {
            throw new InvalidOperationException("Пара уже закончилась.");
        }

        EndsAt = EndsAt.AddMinutes(minutes);
    }

    /// <summary>Завершает пару досрочно. Повторный вызов ничего не меняет.</summary>
    public void Finish(DateTimeOffset now, Guid finishedByUserId)
    {
        if (ClosedAt is not null)
        {
            return;
        }

        ClosedAt = now.ToUniversalTime();
        ClosedByUserId = finishedByUserId;
    }
}
