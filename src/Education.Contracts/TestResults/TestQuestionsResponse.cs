namespace Education.Contracts.TestResults;

/// <summary>
/// Список вопросов текущей попытки тестирования.
/// </summary>
public sealed record TestQuestionsResponse(
    IReadOnlyList<TestQuestionResponse> Questions,
    bool IsCompleted);
