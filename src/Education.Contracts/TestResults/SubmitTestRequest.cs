namespace Education.Contracts.TestResults;

/// <summary>
/// Запрос на отправку ответов текущей попытки тестирования.
/// </summary>
public sealed record SubmitTestRequest(
    IReadOnlyList<SubmitAnswerRequest> Answers);


