namespace Education.Application.Practicals;

/// <summary>
/// Вопрос с признаком выбора для практического материала.
/// </summary>
public sealed record SelectableQuestion(Guid Id, string Text, Guid Type, string Body, double Weight, bool IsSelected);

