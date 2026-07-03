namespace Education.Contracts.Practicals;

/// <summary>
/// Данные задания практического материала.
/// </summary>
public sealed record TaskResponse(
    Guid Id,
    string Name,
    string Text);


