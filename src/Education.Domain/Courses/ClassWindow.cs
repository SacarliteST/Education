using Education.Domain.Common;

namespace Education.Domain.Courses;

/// <summary>
/// «Пара» — окно доступа к тестам и практике курса для одной группы: начинает преподаватель, задаёт длительность.
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

    /// <summary>Группа студентов (как в <c>User.GroupName</c>).</summary>
    public string GroupName { get; private set; } = String.Empty;

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

    /// <summary>Начинает пару на заданную длительность.</summary>
    public static ClassWindow Start(
        Guid courseId, string groupName, DateTimeOffset now, int durationMinutes, Guid startedByUserId)
    {
        if (durationMinutes is < MinDurationMinutes or > MaxDurationMinutes)
        {
            throw new ArgumentOutOfRangeException(nameof(durationMinutes));
        }

        var start = now.ToUniversalTime();
        return new ClassWindow
        {
            CourseId = courseId,
            GroupName = groupName.Trim(),
            StartsAt = start,
            EndsAt = start.AddMinutes(durationMinutes),
            StartedByUserId = startedByUserId,
            CreatedAt = start,
        };
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
