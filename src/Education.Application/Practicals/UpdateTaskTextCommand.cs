namespace Education.Application.Practicals;

/// <summary>
/// Команда обновления текста задания практического материала.
/// </summary>
/// <param name="Text">Новый текст задания.</param>
public sealed record UpdateTaskTextCommand(string Text);
