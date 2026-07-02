namespace Education.Contracts.Questions;

/// <summary>
/// Данные вопроса тестирования, возвращаемые API системы обучения.
/// </summary>
public sealed record QuestionResponse(
    long Id,
    string Text,
    long Type,
    double Weight,
    string Answer);
