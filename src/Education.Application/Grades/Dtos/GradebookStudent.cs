namespace Education.Application.Grades;

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
