namespace Education.Application.TestResults;

/// <summary>
/// Вопрос, выдаваемый студенту в текущей попытке тестирования.
/// </summary>
public sealed record TestQuestion(Guid Id, string Text, Guid Type, string Body);

