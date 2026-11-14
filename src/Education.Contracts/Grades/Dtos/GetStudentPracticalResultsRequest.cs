namespace Education.Contracts.Grades;

/// <summary>
/// Параметры получения детализации результатов студента по практике.
/// </summary>
/// <param name="ClassWindowId">
/// Пара, результатами которой ограничивается детализация, или <see langword="null"/> для всей истории.
/// </param>
public sealed record GetStudentPracticalResultsRequest(Guid? ClassWindowId = null);
