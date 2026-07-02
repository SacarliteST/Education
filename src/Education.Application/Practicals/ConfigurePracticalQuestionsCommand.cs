namespace Education.Application.Practicals;

/// <summary>
/// Команда настройки вопросов и порогов оценивания практического материала.
/// </summary>
public sealed record ConfigurePracticalQuestionsCommand(
    long PracticalId,
    IReadOnlyList<long> QuestionIds,
    int TriesCount,
    double PercentForFive,
    double PercentForFour,
    double PercentForThree);
