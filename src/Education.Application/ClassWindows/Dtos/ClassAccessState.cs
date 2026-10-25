namespace Education.Application.ClassWindows;

/// <summary>Состояние доступа студента к практике курса.</summary>
/// <param name="PracticeOnlyDuringClass">Курс работает в режиме «практика только на парах».</param>
/// <param name="IsOpen">Тесты и практика сейчас доступны.</param>
/// <param name="EndsAt">Конец текущей пары, если она идёт.</param>
public sealed record ClassAccessState(bool PracticeOnlyDuringClass, bool IsOpen, DateTimeOffset? EndsAt);
