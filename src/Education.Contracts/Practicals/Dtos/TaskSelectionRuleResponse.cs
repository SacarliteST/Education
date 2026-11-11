namespace Education.Contracts.Practicals;

/// <summary>Правило случайной выборки заданий практики.</summary>
/// <param name="PracticalId">Практика.</param>
/// <param name="PracticalModuleId">Внешний модуль, из каталога которого выбираются задания.</param>
/// <param name="TaskCount">Количество заданий в наборе студента.</param>
/// <param name="DifficultyLevels">Допустимые уровни сложности; пусто — любые.</param>
/// <param name="TopicRefs">Допустимые темы модуля; пусто — любые.</param>
/// <param name="EmptySelectionWarning">
/// <see langword="true"/>, если по правилу каталог модуля прямо сейчас не даёт ни одного задания
/// (или каталог недоступен) — правило сохранено, но наборы не сформируются.
/// </param>
public sealed record TaskSelectionRuleResponse(
    Guid PracticalId,
    Guid PracticalModuleId,
    int TaskCount,
    IReadOnlyList<short> DifficultyLevels,
    IReadOnlyList<string> TopicRefs,
    bool EmptySelectionWarning);
