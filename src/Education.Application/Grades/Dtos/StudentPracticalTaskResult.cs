namespace Education.Application.Grades;

/// <summary>Результаты студента по заданию внешнего модуля.</summary>
public sealed record StudentPracticalTaskResult(
    Guid? AssignmentId,
    Guid? PracticalTaskId,
    int SortOrder,
    string ExternalTaskRef,
    string? TaskName,
    Guid? ClassWindowId,
    IReadOnlyList<StudentPracticalSessionResult> Sessions)
{
    /// <summary>Количество запусков задания.</summary>
    public int AttemptsCount => Sessions.Count;

    /// <summary>Лучшая зафиксированная завершённая сессия задания.</summary>
    public StudentPracticalSessionResult? RecordedResult => Sessions
        .Where(session => session.Status == Domain.PracticalModules.ModuleSessionState.Completed
            && session.Grade is not null)
        .OrderByDescending(session => session.Grade)
        .ThenByDescending(session => session.EndedAt)
        .FirstOrDefault();

    /// <summary>Текущее состояние прохождения задания.</summary>
    public StudentPracticalTaskResultState ResultState => Sessions.Any(session =>
        session.Status == Domain.PracticalModules.ModuleSessionState.Active)
            ? StudentPracticalTaskResultState.InProgress
            : RecordedResult is not null
                ? StudentPracticalTaskResultState.Completed
                : Sessions.Count == 0
                    ? StudentPracticalTaskResultState.NotStarted
                    : StudentPracticalTaskResultState.Expired;
}
