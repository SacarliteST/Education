namespace Education.Contracts.Grades;

/// <summary>
/// Детализация результатов студента по практическому материалу.
/// </summary>
/// <param name="StudentId">Идентификатор студента.</param>
/// <param name="StudentName">Отображаемое имя студента.</param>
/// <param name="PracticalId">Идентификатор практического материала.</param>
/// <param name="Kind"><c>external</c> для внешнего модуля или <c>internal</c> для внутреннего теста.</param>
/// <param name="ClassWindowId">Выбранная пара или <see langword="null"/>, если возвращается вся история.</param>
/// <param name="PracticalModuleId">Внешний практический модуль или <see langword="null"/> для внутренней практики.</param>
/// <param name="Tasks">Задания внешнего модуля; для внутренней практики массив пуст.</param>
/// <param name="TestAttempts">Попытки внутреннего теста; для внешней практики массив пуст.</param>
public sealed record StudentPracticalResultsResponse(
    Guid StudentId,
    string StudentName,
    Guid PracticalId,
    string Kind,
    Guid? ClassWindowId,
    Guid? PracticalModuleId,
    IReadOnlyList<StudentPracticalTaskResultResponse> Tasks,
    IReadOnlyList<StudentTestAttemptResultResponse> TestAttempts);
