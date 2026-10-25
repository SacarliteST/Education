namespace Education.Application.ClassWindows;

/// <summary>Сведения о курсе практики для проверки доступа.</summary>
public sealed record PracticeGate(Guid CourseId, bool PracticeOnlyDuringClass);
