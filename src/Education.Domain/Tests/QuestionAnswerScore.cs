namespace Education.Domain.Tests;

public sealed record QuestionAnswerScore(
    long QuestionId,
    string QuestionText,
    double QuestionWeight,
    double QuestionScore,
    string UserAnswer,
    bool IsCorrect);
