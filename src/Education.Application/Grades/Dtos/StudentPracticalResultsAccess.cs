namespace Education.Application.Grades;

/// <summary>
/// Данные для проверки доступа преподавателя к детализации результатов студента.
/// </summary>
public sealed record StudentPracticalResultsAccess(
    Guid CourseId,
    Guid TeacherId,
    bool IsStudentAssigned,
    string StudentName);
