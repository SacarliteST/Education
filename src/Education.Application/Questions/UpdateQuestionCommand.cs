namespace Education.Application.Questions;

/// <summary>
/// Команда обновления вопроса тестирования.
/// </summary>
public sealed record UpdateQuestionCommand(string Text, string Body, string Answer, double Weight, long Type);
