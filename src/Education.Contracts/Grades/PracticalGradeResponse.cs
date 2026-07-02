namespace Education.Contracts.Grades;

/// <summary>
/// Итоговая оценка за практический материал или список условий для её получения.
/// </summary>
public sealed record PracticalGradeResponse(
    int? Grade,
    IReadOnlyList<string> Messages);
