namespace Education.Application.Practicals.TaskAssignments;

/// <summary>
/// Конфликт режимов практики: у практики с правилом случайной выборки нельзя менять
/// ручной список заданий <c>Case</c>, а практике с ручным списком нельзя задать правило.
/// </summary>
public sealed class TaskSelectionModeConflictException(string message) : Exception(message);
