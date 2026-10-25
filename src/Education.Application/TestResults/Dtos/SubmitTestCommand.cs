namespace Education.Application.TestResults;

/// <summary>
/// Команда отправки ответов текущей попытки тестирования.
/// </summary>
public sealed record SubmitTestCommand(Guid PracticalId, IReadOnlyList<SubmitAnswerCommand> Answers);

