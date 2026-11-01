namespace Education.Contracts.ClassWindows;

/// <summary>Запрос запуска пары.</summary>
/// <param name="GroupName">Группа студентов; <see langword="null"/> — пара только для перечисленных студентов.</param>
/// <param name="StudentUserIds">Студенты с точечным доступом; нужна группа и/или хотя бы один студент.</param>
/// <param name="DurationMinutes">Длительность, минут (5–480).</param>
public sealed record StartClassWindowRequest(string? GroupName, IReadOnlyList<Guid>? StudentUserIds, int DurationMinutes);
