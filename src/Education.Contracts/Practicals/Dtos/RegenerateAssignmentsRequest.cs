namespace Education.Contracts.Practicals;

/// <summary>Запрос пересбора набора заданий студента на пару (преподаватель, только до первой сессии).</summary>
/// <param name="StudentId">Идентификатор студента.</param>
/// <param name="ClassWindowId">Идентификатор пары.</param>
public sealed record RegenerateAssignmentsRequest(Guid StudentId, Guid ClassWindowId);
