namespace Education.Application.Practicals.TaskAssignments;

/// <summary>Нельзя пересобрать набор заданий студента: по его назначениям уже есть сессии модуля.</summary>
public sealed class AssignmentRegenerationForbiddenException()
    : Exception("По набору студента уже есть сессии модуля — пересбор невозможен.");
