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

/// <summary>Практика в ведомости курса.</summary>
public sealed record GradebookPractical(Guid Id, string Name, bool IsExternal, Guid ModuleId, string ModuleName);

/// <summary>Ячейка ведомости: студент × практика. <c>Assigned = false</c> — практика студенту не назначена.</summary>
public sealed record GradebookCell(Guid PracticalId, bool Assigned, PracticalGrade? Grade)
{
    public GradeStatus Status => Grade is null ? GradeStatus.Incomplete : PracticalGradeCalculator.StatusOf(Grade.Grade);
}

/// <summary>Строка ведомости — студент и его ячейки в порядке <see cref="Gradebook.Practicals"/>.</summary>
public sealed record GradebookStudent(
    Guid StudentId,
    string StudentName,
    string Login,
    string? Group,
    IReadOnlyList<GradebookCell> Cells)
{
    /// <summary>Назначенных практик.</summary>
    public int AssignedCount => Cells.Count(cell => cell.Assigned);

    /// <summary>Практик с оценкой не ниже проходной.</summary>
    public int PassedCount => Cells.Count(cell => cell.Assigned && cell.Status == GradeStatus.Passed);

    /// <summary>Средняя оценка по практикам, где она уже есть; нет ни одной — <c>null</c>.</summary>
    public double? AverageGrade
    {
        get
        {
            var grades = Cells.Where(cell => cell.Assigned && cell.Grade?.Grade is not null)
                .Select(cell => (double)cell.Grade!.Grade!.Value)
                .ToList();
            return grades.Count == 0 ? null : Math.Round(grades.Average(), 2);
        }
    }
}

/// <summary>Ведомость курса: практики и студенты с оценками.</summary>
public sealed record Gradebook(IReadOnlyList<GradebookPractical> Practicals, IReadOnlyList<GradebookStudent> Students);
