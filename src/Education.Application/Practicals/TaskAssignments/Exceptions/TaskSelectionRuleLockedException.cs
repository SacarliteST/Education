namespace Education.Application.Practicals.TaskAssignments;

/// <summary>Нельзя удалить правило выборки: по назначениям практики уже есть сессии модуля.</summary>
public sealed class TaskSelectionRuleLockedException()
    : Exception("По назначениям практики уже есть сессии модуля — правило нельзя удалить.");
