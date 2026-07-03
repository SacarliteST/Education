namespace Education.Application.TestResults;

/// <summary>
/// Вопросы текущей попытки тестирования.
/// </summary>
public sealed record TestQuestions(IReadOnlyList<TestQuestion> Questions, bool IsCompleted);

