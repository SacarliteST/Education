namespace Education.Contracts.Questions;

/// <summary>
/// Данные вопроса тестирования, возвращаемые API системы обучения.
/// </summary>
public sealed record QuestionResponse(
    Guid Id,
    string Text,
    Guid Type,
    double Weight,
    string Answer);


