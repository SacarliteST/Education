namespace Education.Application.Questions;

/// <summary>
/// Команда создания вопроса тестирования.
/// </summary>
public sealed record CreateQuestionCommand(long ModuleId, long Type, string Text, string Body, string Answer, double Weight);
