namespace Education.Contracts.Practicals;

/// <summary>
/// Вопрос с признаком выбора для практического материала.
/// </summary>
public sealed record SelectableQuestionResponse(
    long Id,
    string Text,
    long Type,
    string Body,
    bool IsSelected);
