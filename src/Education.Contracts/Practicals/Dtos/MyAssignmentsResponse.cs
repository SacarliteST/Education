namespace Education.Contracts.Practicals;

/// <summary>Набор заданий текущего студента по практике на пару.</summary>
/// <param name="ClassWindowId">Пара, к которой относится набор.</param>
/// <param name="Tasks">Набор заданий студента.</param>
public sealed record MyAssignmentsResponse(
    Guid ClassWindowId,
    IReadOnlyList<AssignmentTaskResponse> Tasks);
