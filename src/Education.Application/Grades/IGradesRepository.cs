namespace Education.Application.Grades;

/// <summary>
/// Предоставляет данные для расчёта оценок.
/// </summary>
public interface IGradesRepository
{
    /// <summary>
    /// Возвращает итоговую оценку за практический материал.
    /// </summary>
    Task<PracticalGrade> GetPracticalGradeAsync(long practicalId, long studentUserId, CancellationToken cancellationToken = default);
}
