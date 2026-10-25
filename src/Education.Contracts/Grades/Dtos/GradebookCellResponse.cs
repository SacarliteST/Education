namespace Education.Contracts.Grades;

/// <summary>Ячейка ведомости: студент × практика.</summary>
/// <param name="PracticalId">Практика.</param>
/// <param name="Assigned"><c>false</c> — практика студенту не назначена.</param>
/// <param name="Grade">Оценка 2–5 или <c>null</c>.</param>
/// <param name="Status"><c>passed</c> / <c>failed</c> / <c>incomplete</c>.</param>
public sealed record GradebookCellResponse(Guid PracticalId, bool Assigned, int? Grade, string Status);
