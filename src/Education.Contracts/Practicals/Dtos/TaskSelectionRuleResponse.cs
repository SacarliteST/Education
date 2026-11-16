namespace Education.Contracts.Practicals;

/// <summary>Правило случайной выборки заданий практики.</summary>
/// <param name="PracticalId">Практика.</param>
/// <param name="PracticalModuleId">Внешний модуль, из каталога которого выбираются задания.</param>
/// <param name="TaskCount">Количество заданий в наборе студента.</param>
/// <param name="DifficultyLevels">Допустимые уровни сложности; пусто — любые.</param>
/// <param name="TopicRefs">Допустимые темы модуля; пусто — любые.</param>
/// <param name="DifficultyQuotas">Точное количество заданий каждого уровня сложности; пусто — точные квоты не заданы.</param>
/// <param name="EmptySelectionWarning">
/// <see langword="true"/>, если в каталоге модуля прямо сейчас недостаточно заданий
/// для формирования полного набора (или каталог недоступен). Правило при этом сохранено.
/// </param>
public sealed record TaskSelectionRuleResponse(
    Guid PracticalId,
    Guid PracticalModuleId,
    int TaskCount,
    IReadOnlyList<short> DifficultyLevels,
    IReadOnlyList<string> TopicRefs,
    IReadOnlyList<TaskDifficultyQuota> DifficultyQuotas,
    bool EmptySelectionWarning);
