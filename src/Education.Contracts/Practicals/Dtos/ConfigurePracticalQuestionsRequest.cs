namespace Education.Contracts.Practicals;

/// <summary>
/// Запрос на настройку вопросов и порогов оценивания практического материала.
/// </summary>
public sealed record ConfigurePracticalQuestionsRequest(
    IReadOnlyList<Guid> QuestionIds,
    int TriesCount,
    double PercentForFive,
    double PercentForFour,
    double PercentForThree);


