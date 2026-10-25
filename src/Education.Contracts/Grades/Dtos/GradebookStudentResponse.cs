namespace Education.Contracts.Grades;

/// <summary>Строка ведомости — студент и его ячейки в порядке практик ведомости.</summary>
public sealed record GradebookStudentResponse(
    Guid StudentId,
    string StudentName,
    string Login,
    string? Group,
    IReadOnlyList<GradebookCellResponse> Cells,
    int AssignedCount,
    int PassedCount,
    double? AverageGrade);
