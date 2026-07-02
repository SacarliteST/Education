namespace Education.Application.TestResults;

/// <summary>
/// Вопрос, выдаваемый студенту в текущей попытке тестирования.
/// </summary>
public sealed record TestQuestion(long Id, string Text, long Type, string Body);
