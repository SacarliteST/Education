namespace Education.Contracts.Practicals;

/// <summary>Запрос создания/замены правила случайной выборки заданий практики.</summary>
/// <param name="PracticalModuleId">Внешний модуль, из каталога которого выбираются задания.</param>
/// <param name="TaskCount">Количество заданий в наборе студента (1–10).</param>
/// <param name="DifficultyLevels">Допустимые уровни сложности модуля; пусто — любые.</param>
/// <param name="TopicRefs">Допустимые темы модуля (идентификаторы тем из каталога); пусто — любые.</param>
/// <param name="DifficultyQuotas">
/// Точное количество заданий каждого уровня сложности. Сумма должна совпадать с <paramref name="TaskCount"/>.
/// Нельзя передавать одновременно с <paramref name="DifficultyLevels"/>.
/// </param>
public sealed record UpsertTaskSelectionRuleRequest(
    Guid PracticalModuleId,
    int TaskCount,
    IReadOnlyList<short>? DifficultyLevels,
    IReadOnlyList<string>? TopicRefs,
    IReadOnlyList<TaskDifficultyQuota>? DifficultyQuotas = null);
