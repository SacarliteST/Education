namespace Education.Contracts.Questions;

/// <summary>
/// Запрос на обновление вопроса тестирования.
/// </summary>
public sealed record UpdateQuestionRequest(
    string Text,
    string Body,
    string Answer,
    double Weight,
    long Type);
