namespace Education.Application.Practicals;

/// <summary>
/// Вопрос с признаком выбора для практического материала.
/// </summary>
public sealed record SelectableQuestion(long Id, string Text, long Type, string Body, bool IsSelected);
