namespace Education.Application.Grades;

/// <summary>Оценка одного студента по практике (для списка преподавателя).</summary>
public sealed record StudentPracticalGrade(
    Guid StudentId,
    string StudentName,
    string Login,
    string? Group,
    PracticalGrade Grade)
{
    public GradeStatus Status => PracticalGradeCalculator.StatusOf(Grade.Grade);
}
