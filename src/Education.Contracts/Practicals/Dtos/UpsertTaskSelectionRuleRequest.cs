namespace Education.Contracts.Practicals;

/// <summary>Запрос создания/замены правила случайной выборки заданий практики.</summary>
/// <param name="PracticalModuleId">Внешний модуль, из каталога которого выбираются задания.</param>
/// <param name="TaskCount">Количество заданий в наборе студента (1–10).</param>
/// <param name="DifficultyLevels">Допустимые уровни сложности модуля; пусто — любые.</param>
/// <param name="TopicRefs">Допустимые темы модуля (идентификаторы тем из каталога); пусто — любые.</param>
public sealed record UpsertTaskSelectionRuleRequest(
    Guid PracticalModuleId,
    int TaskCount,
    IReadOnlyList<short>? DifficultyLevels,
    IReadOnlyList<string>? TopicRefs);
