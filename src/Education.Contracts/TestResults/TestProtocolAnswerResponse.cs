namespace Education.Contracts.TestResults;

/// <summary>
/// Детализация ответа в протоколе попытки тестирования.
/// </summary>
/// <param name="QuestionType">Идентификатор типа вопроса (как в <c>TestQuestionResponse.Type</c>).</param>
/// <param name="QuestionBody">Варианты ответа / доп. данные вопроса — для расшифровки <c>UserAnswer</c>.</param>
public sealed record TestProtocolAnswerResponse(
    Guid QuestionId,
    string QuestionText,
    double QuestionWeight,
    double QuestionScore,
    string UserAnswer,
    bool IsCorrect,
    Guid QuestionType,
    string QuestionBody);


