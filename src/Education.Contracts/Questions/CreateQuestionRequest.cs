namespace Education.Contracts.Questions;

/// <summary>
/// Запрос на создание вопроса тестирования.
/// </summary>
public sealed record CreateQuestionRequest(
    long ModuleId,
    long Type,
    string Text,
    string Body,
    string Answer,
    double Weight);
