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
/// <param name="Sessions">Сессии задания от новых к старым.</param>
public sealed record StudentPracticalTaskResultResponse(
    Guid? AssignmentId,
    Guid? PracticalTaskId,
    int SortOrder,
    string ExternalTaskRef,
    string? TaskName,
    Guid? ClassWindowId,
    IReadOnlyList<StudentPracticalSessionResultResponse> Sessions);
