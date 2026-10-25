namespace Education.Contracts.Questions;

/// <summary>
/// Запрос на создание вопроса тестирования.
/// </summary>
public sealed record CreateQuestionRequest(
    Guid ModuleId,
    Guid Type,
    string Text,
    string Body,
    string Answer,
    double Weight);


