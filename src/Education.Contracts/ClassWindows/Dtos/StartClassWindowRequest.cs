namespace Education.Contracts.ClassWindows;

/// <summary>Запрос запуска пары.</summary>
/// <param name="GroupName">Группа студентов.</param>
/// <param name="DurationMinutes">Длительность, минут (5–480).</param>
public sealed record StartClassWindowRequest(string GroupName, int DurationMinutes);
