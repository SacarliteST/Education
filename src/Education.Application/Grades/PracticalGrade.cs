namespace Education.Application.Grades;

/// <summary>
/// Итоговая оценка за практический материал или условия для её получения.
/// </summary>
public sealed record PracticalGrade(int? Grade, IReadOnlyList<string> Messages);
