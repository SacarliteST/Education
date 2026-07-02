namespace Education.Contracts.Practicals;

/// <summary>
/// Данные задания практического материала.
/// </summary>
public sealed record TaskResponse(
    long Id,
    string Name,
    string Text);
