namespace Education.Application.Theories;

/// <summary>
/// Команда обновления заголовка теоретического материала.
/// </summary>
/// <param name="Title">Новый заголовок теоретического материала.</param>
public sealed record UpdateTheoryTitleCommand(string Title);

