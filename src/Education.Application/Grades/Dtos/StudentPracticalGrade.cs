namespace Education.Application.Grades;

/// <summary>Оценка одного студента по практике (для списка преподавателя).</summary>
public sealed record StudentPracticalGrade(
    Guid StudentId,
    string StudentName,
    string Login,
    string? Group,
    PracticalGrade Grade)
{
    /// <summary>Статус оценки: не выполнена, зачтена или не зачтена.</summary>
    public GradeStatus Status => PracticalGradeCalculator.StatusOf(Grade.Grade);
}
