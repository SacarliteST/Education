namespace Education.Contracts.Practicals;

/// <summary>
/// Данные практического материала, возвращаемые API системы обучения.
/// </summary>
public sealed record PracticalResponse(
    long Id,
    string Name);
