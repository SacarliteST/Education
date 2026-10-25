namespace Education.Application.Questions;

/// <summary>
/// Команда создания вопроса тестирования.
/// </summary>
public sealed record CreateQuestionCommand(Guid ModuleId, Guid Type, string Text, string Body, string Answer, double Weight);

