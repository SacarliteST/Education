namespace Education.Domain.Tests;

/// <summary>
/// Результат проверки ответа пользователя на вопрос.
/// </summary>
/// <param name="QuestionId">Идентификатор вопроса.</param>
/// <param name="QuestionText">Текст вопроса.</param>
/// <param name="QuestionWeight">Вес вопроса.</param>
/// <param name="QuestionScore">Набранный балл за вопрос.</param>
/// <param name="UserAnswer">Ответ пользователя.</param>
/// <param name="IsCorrect">Признак полностью правильного ответа.</param>
public sealed record QuestionAnswerScore(
    Guid QuestionId,
    string QuestionText,
    double QuestionWeight,
    double QuestionScore,
    string UserAnswer,
    bool IsCorrect);

