using Education.Application.PracticalModules;
using Education.Domain.Practicals;

namespace Education.Application.Practicals.TaskAssignments;

/// <summary>Отбор заданий каталога модуля по правилу выборки.</summary>
internal static class TaskSelectionFilter
{
    /// <summary>
    /// Подходит ли задание каталога под правило: пустые списки правила — любые значения;
    /// задание без метаданных при заданном фильтре исключается.
    /// </summary>
    /// <param name="rule">Правило выборки.</param>
    /// <param name="task">Задание из каталога модуля.</param>
    public static bool Matches(TaskSelectionRule rule, ModuleCatalogTask task) =>
        (rule.DifficultyDistribution.Count == 0
            ? rule.DifficultyLevels.Count == 0
                || (task.DifficultyLevel is { } allowedLevel && rule.DifficultyLevels.Contains(allowedLevel))
            : task.DifficultyLevel is { } quotaLevel && rule.DifficultyDistribution.Contains(quotaLevel))
        && (rule.TopicRefs.Count == 0
            || (task.TopicId is { } topicId && rule.TopicRefs.Contains(topicId)));
}
