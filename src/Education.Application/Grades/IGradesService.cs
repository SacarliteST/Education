namespace Education.Application.Grades;

/// <summary>
/// Выполняет сценарии расчёта оценок.
/// </summary>
public interface IGradesService
{
    /// <summary>
    /// Возвращает итоговую оценку за практический материал для текущего студента.
    /// </summary>
    Task<PracticalGrade> GetPracticalGradeAsync(long practicalId, CancellationToken cancellationToken = default);
}
