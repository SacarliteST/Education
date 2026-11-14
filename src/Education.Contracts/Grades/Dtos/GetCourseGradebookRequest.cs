namespace Education.Contracts.Grades;

/// <summary>
/// Параметры получения ведомости курса.
/// </summary>
/// <param name="ClassWindowId">
/// Пара, результатами которой ограничивается ведомость, или <see langword="null"/> для агрегации за всё время.
/// </param>
public sealed record GetCourseGradebookRequest(Guid? ClassWindowId = null);
