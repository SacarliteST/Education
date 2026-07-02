namespace Education.Contracts.TestResults;

/// <summary>
/// Вопрос, выдаваемый студенту в текущей попытке тестирования.
/// </summary>
public sealed record TestQuestionResponse(
    long Id,
    string Text,
    long Type,
    string Body);
