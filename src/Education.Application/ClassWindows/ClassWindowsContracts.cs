using Education.Domain.Courses;
using Education.Domain.PracticalModules;

namespace Education.Application.ClassWindows;

/// <summary>Пара в виде для API.</summary>
public sealed record ClassWindowInfo(
    Guid Id,
    Guid CourseId,
    string GroupName,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt,
    DateTimeOffset? ClosedAt,
    bool IsActive);

/// <summary>Режим курса, группы студентов и пары (для вкладки «Пары»).</summary>
public sealed record CourseClassWindows(
    bool PracticeOnlyDuringClass,
    IReadOnlyList<string> Groups,
    IReadOnlyList<ClassWindowInfo> Windows);

/// <summary>Состояние доступа студента к практике курса.</summary>
/// <param name="PracticeOnlyDuringClass">Курс работает в режиме «практика только на парах».</param>
/// <param name="IsOpen">Тесты и практика сейчас доступны.</param>
/// <param name="EndsAt">Конец текущей пары, если она идёт.</param>
public sealed record ClassAccessState(bool PracticeOnlyDuringClass, bool IsOpen, DateTimeOffset? EndsAt);

/// <summary>Сведения о курсе практики для проверки доступа.</summary>
public sealed record PracticeGate(Guid CourseId, bool PracticeOnlyDuringClass);

/// <summary>Идущая сессия внешнего модуля вместе с данными для повторного пуша.</summary>
public sealed record ActiveSessionTarget(
    PracticalModuleSession Session,
    Guid PracticalModuleId,
    string ExternalTaskRef,
    int? AttemptsPerSession,
    int? TimeLimitMinutes);

/// <summary>Тесты и практика недоступны: пара группы не идёт.</summary>
public sealed class ClassWindowClosedException() : Exception("Практика и тесты доступны только во время пары.");

/// <summary>Для курса и группы уже идёт пара.</summary>
public sealed class ClassWindowAlreadyOpenException() : Exception("Для этой группы пара уже идёт.");

/// <summary>Пара не найдена.</summary>
public sealed class ClassWindowNotFoundException() : Exception("Пара не найдена.");

/// <summary>Пара уже закончилась.</summary>
public sealed class ClassWindowAlreadyFinishedException() : Exception("Пара уже закончилась.");

/// <summary>У студента не указана группа — начать пару для него невозможно.</summary>
public sealed class ClassWindowGroupRequiredException() : Exception("Группа не указана.");
