namespace Education.Application.Practicals;

/// <summary>
/// Данные настройки теста практического материала.
/// </summary>
public sealed record PracticalQuestionsSetup(
    IReadOnlyList<SelectableQuestion> Questions,
    bool IsPublic,
    int TriesCount,
    double PercentForFive,
    double PercentForFour,
    double PercentForThree);

