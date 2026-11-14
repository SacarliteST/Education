namespace Education.Application.Grades;

/// <summary>Результаты студента по заданию внешнего модуля.</summary>
public sealed record StudentPracticalTaskResult(
    Guid? AssignmentId,
    Guid? PracticalTaskId,
    int SortOrder,
    string ExternalTaskRef,
    string? TaskName,
    Guid? ClassWindowId,
    IReadOnlyList<StudentPracticalSessionResult> Sessions);
