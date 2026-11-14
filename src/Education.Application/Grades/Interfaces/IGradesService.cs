namespace Education.Application.Grades;

/// <summary>
/// Выполняет сценарии расчёта оценок.
/// </summary>
public interface IGradesService
{
    /// <summary>
    /// Возвращает итоговую оценку за практический материал для текущего студента.
    /// </summary>
    Task<PracticalGrade> GetPracticalGradeAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает оценки студентов по практике для преподавателя — владельца курса практики.
    /// <c>null</c> — практика не найдена или не принадлежит преподавателю.
    /// </summary>
    Task<IReadOnlyList<StudentPracticalGrade>?> GetPracticalStudentGradesAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает ведомость курса для преподавателя — владельца курса.
    /// </summary>
    /// <exception cref="Courses.CourseAccessDeniedException">Курс не принадлежит преподавателю.</exception>
    Task<Gradebook> GetCourseGradebookAsync(
        Guid courseId,
        Guid? classWindowId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает детализацию результатов студента по практике для преподавателя-владельца курса.
    /// </summary>
    Task<StudentPracticalResults> GetStudentPracticalResultsAsync(
        Guid practicalId,
        Guid studentId,
        Guid? classWindowId = null,
        CancellationToken cancellationToken = default);
}

