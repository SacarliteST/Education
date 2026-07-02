namespace Education.Application.TestResults;

/// <summary>
/// Команда отправки ответов текущей попытки тестирования.
/// </summary>
public sealed record SubmitTestCommand(long PracticalId, IReadOnlyList<SubmitAnswerCommand> Answers);
