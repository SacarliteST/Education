namespace Education.Application.TestResults;

/// <summary>
/// Ответ в протоколе попытки, обогащённый типом и телом вопроса — чтобы UI мог
/// показать текст выбранного варианта, а не его идентификатор (<c>TD-010</c>).
/// </summary>
public sealed record TestProtocolAnswer(
    Guid QuestionId,
    string QuestionText,
    double QuestionWeight,
    double QuestionScore,
    string UserAnswer,
    bool IsCorrect,
    Guid QuestionType,
    string QuestionBody);
