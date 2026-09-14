namespace Education.Contracts.Practicals;

/// <summary>
/// Вопрос с признаком выбора для практического материала.
/// </summary>
public sealed record SelectableQuestionResponse(
    Guid Id,
    string Text,
    Guid Type,
    string Body,
    double Weight,
    bool IsSelected);


