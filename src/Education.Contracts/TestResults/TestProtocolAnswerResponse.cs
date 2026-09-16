namespace Education.Contracts.TestResults;

/// <summary>
/// Детализация ответа в протоколе попытки тестирования.
/// </summary>
/// <param name="QuestionId">Идентификатор вопроса.</param>
/// <param name="QuestionText">Текст вопроса.</param>
/// <param name="QuestionWeight">Вес вопроса — максимальный балл за полностью верный ответ.</param>
/// <param name="QuestionScore">Фактически начисленный балл за ответ студента в этой попытке.</param>
/// <param name="UserAnswer">Ответ студента в закодированном виде — для расшифровки нужны <paramref name="QuestionType"/> и <paramref name="QuestionBody"/>.</param>
/// <param name="IsCorrect">Засчитан ли ответ как полностью верный (<c>QuestionScore == QuestionWeight</c>).</param>
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


