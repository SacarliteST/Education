namespace Education.Application.Grades;

/// <summary>Ячейка ведомости: студент × практика. <c>Assigned = false</c> — практика студенту не назначена.</summary>
public sealed record GradebookCell(Guid PracticalId, bool Assigned, PracticalGrade? Grade)
{
    public GradeStatus Status => Grade is null ? GradeStatus.Incomplete : PracticalGradeCalculator.StatusOf(Grade.Grade);
}
