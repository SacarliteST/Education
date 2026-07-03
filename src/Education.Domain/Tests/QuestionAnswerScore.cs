namespace Education.Domain.Tests;

public sealed record QuestionAnswerScore(
    Guid QuestionId,
    string QuestionText,
    double QuestionWeight,
    double QuestionScore,
    string UserAnswer,
    bool IsCorrect);

