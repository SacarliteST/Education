namespace Education.Application.TestResults;

/// <summary>
/// Ответ студента на вопрос тестирования.
/// </summary>
public sealed record SubmitAnswerCommand(Guid QuestionId, string Answer);

