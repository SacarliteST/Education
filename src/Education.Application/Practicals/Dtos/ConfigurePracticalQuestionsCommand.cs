namespace Education.Application.Practicals;

/// <summary>
/// Команда настройки вопросов и порогов оценивания практического материала.
/// </summary>
public sealed record ConfigurePracticalQuestionsCommand(
    Guid PracticalId,
    IReadOnlyList<Guid> QuestionIds,
    int TriesCount,
    double PercentForFive,
    double PercentForFour,
    double PercentForThree);

