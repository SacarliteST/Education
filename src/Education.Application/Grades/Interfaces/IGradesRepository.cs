namespace Education.Application.Grades;

/// <summary>
/// Предоставляет данные для расчёта оценок.
/// </summary>
public interface IGradesRepository
{
    /// <summary>
    /// Возвращает итоговую оценку за практический материал.
    /// </summary>
    Task<PracticalGrade> GetPracticalGradeAsync(Guid practicalId, Guid studentUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает оценки всех студентов, которым назначена практика (по тому же правилу, что и для самого студента).
    /// </summary>
    Task<IReadOnlyList<StudentPracticalGrade>> GetPracticalStudentGradesAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает ведомость курса: все практики курса и студентов (назначенных на курс или на любую его практику).
    /// </summary>
    Task<Gradebook> GetCourseGradebookAsync(Guid courseId, CancellationToken cancellationToken = default);
}

