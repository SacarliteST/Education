namespace Education.Application.Theories;

/// <summary>
/// Команда обновления текста теоретического материала.
/// </summary>
/// <param name="Text">Новый текст теоретического материала.</param>
public sealed record UpdateTheoryTextCommand(string Text);

