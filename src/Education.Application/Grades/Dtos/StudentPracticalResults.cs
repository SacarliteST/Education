using Education.Domain.Practicals;

namespace Education.Application.Grades;

/// <summary>Детализация результатов студента по практическому материалу.</summary>
public sealed record StudentPracticalResults(
    Guid StudentId,
    string StudentName,
    Guid PracticalId,
    PracticalKind Kind,
    Guid? ClassWindowId,
    Guid? PracticalModuleId,
    IReadOnlyList<StudentPracticalTaskResult> Tasks,
    IReadOnlyList<StudentTestAttemptResult> TestAttempts);
