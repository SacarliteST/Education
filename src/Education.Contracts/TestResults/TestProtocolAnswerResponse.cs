namespace Education.Contracts.TestResults;

/// <summary>
/// Детализация ответа в протоколе попытки тестирования.
/// </summary>
public sealed record TestProtocolAnswerResponse(
    Guid QuestionId,
    string QuestionText,
    double QuestionWeight,
    double QuestionScore,
    string UserAnswer,
    bool IsCorrect);


