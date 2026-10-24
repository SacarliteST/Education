namespace Education.Contracts.ClassWindows;

/// <summary>Запрос запуска пары.</summary>
/// <param name="GroupName">Группа студентов.</param>
/// <param name="DurationMinutes">Длительность, минут (5–480).</param>
public sealed record StartClassWindowRequest(string GroupName, int DurationMinutes);

/// <summary>Запрос продления пары.</summary>
/// <param name="Minutes">На сколько минут продлить (1–240).</param>
public sealed record ExtendClassWindowRequest(int Minutes);

/// <summary>Запрос смены режима курса.</summary>
public sealed record SetPracticeAccessRequest(bool PracticeOnlyDuringClass);

/// <summary>Пара.</summary>
public sealed record ClassWindowResponse(
    Guid Id,
    Guid CourseId,
    string GroupName,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    DateTimeOffset? ClosedAt,
    bool IsActive);

/// <summary>Режим курса, группы студентов и пары.</summary>
public sealed record CourseClassWindowsResponse(
    bool PracticeOnlyDuringClass,
    IReadOnlyList<string> Groups,
    IReadOnlyList<ClassWindowResponse> Windows);

/// <summary>Доступ студента к практике курса.</summary>
public sealed record ClassAccessResponse(bool PracticeOnlyDuringClass, bool IsOpen, DateTimeOffset? EndsAt);
