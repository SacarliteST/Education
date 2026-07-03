namespace Education.Contracts.TestResults;

/// <summary>
/// Ответ студента на вопрос тестирования.
/// </summary>
public sealed record SubmitAnswerRequest(
    Guid Id,
    string Answer);


