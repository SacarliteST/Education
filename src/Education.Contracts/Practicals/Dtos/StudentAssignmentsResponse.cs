namespace Education.Contracts.Practicals;

/// <summary>Строка таблицы «студент → задания» по практике на пару (для преподавателя).</summary>
/// <param name="StudentId">Идентификатор студента.</param>
/// <param name="StudentName">Отображаемое имя студента.</param>
/// <param name="Tasks">Набор заданий студента на пару.</param>
public sealed record StudentAssignmentsResponse(
    Guid StudentId,
    string StudentName,
    IReadOnlyList<AssignmentTaskResponse> Tasks);
