namespace Education.Contracts.Grades;

/// <summary>
/// Результаты студента по одному заданию внешнего практического модуля.
/// </summary>
/// <param name="AssignmentId">Назначение случайного набора или <see langword="null"/> для фиксированного задания.</param>
/// <param name="PracticalTaskId">Фиксированное задание практики или <see langword="null"/> для случайного назначения.</param>
/// <param name="SortOrder">Порядок задания в наборе.</param>
/// <param name="ExternalTaskRef">Стабильный идентификатор задания во внешнем модуле.</param>
/// <param name="TaskName">Название из каталога модуля или <see langword="null"/>, если каталог недоступен.</param>
/// <param name="ClassWindowId">Пара случайного назначения или <see langword="null"/> для фиксированного задания.</param>
/// <param name="ResultStatus"><c>notStarted</c>, <c>inProgress</c>, <c>completed</c> или <c>expired</c>.</param>
/// <param name="AttemptsCount">Количество запусков задания.</param>
/// <param name="ResultScore">Лучший зафиксированный балл 0–100 или <see langword="null"/>, если результата нет.</param>
/// <param name="ResultSessionId">Сессия лучшего результата или <see langword="null"/>, если результата нет.</param>
/// <param name="Sessions">Сессии задания от новых к старым.</param>
public sealed record StudentPracticalTaskResultResponse(
    Guid? AssignmentId,
    Guid? PracticalTaskId,
    int SortOrder,
    string ExternalTaskRef,
    string? TaskName,
    Guid? ClassWindowId,
    string ResultStatus,
    int AttemptsCount,
    int? ResultScore,
    Guid? ResultSessionId,
    IReadOnlyList<StudentPracticalSessionResultResponse> Sessions);
