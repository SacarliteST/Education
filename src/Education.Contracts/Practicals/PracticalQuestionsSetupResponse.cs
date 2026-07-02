namespace Education.Contracts.Practicals;

/// <summary>
/// Данные для настройки теста практического материала.
/// </summary>
public sealed record PracticalQuestionsSetupResponse(
    IReadOnlyList<SelectableQuestionResponse> Questions,
    bool IsPublic,
    int TriesCount,
    double PercentForFive,
    double PercentForFour,
    double PercentForThree);
