namespace Education.Contracts.Practicals;

/// <summary>Точная квота заданий одного уровня сложности в случайном наборе.</summary>
/// <param name="DifficultyLevel">Уровень сложности по шкале модуля.</param>
/// <param name="TaskCount">Количество заданий этого уровня.</param>
public sealed record TaskDifficultyQuota(short DifficultyLevel, int TaskCount);
