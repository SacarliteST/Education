namespace Education.Contracts.TestResults;

/// <summary>
/// Вопрос, выдаваемый студенту в текущей попытке тестирования.
/// </summary>
public sealed record TestQuestionResponse(
    Guid Id,
    string Text,
    Guid Type,
    string Body);


