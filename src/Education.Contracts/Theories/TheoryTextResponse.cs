namespace Education.Contracts.Theories;

/// <summary>
/// Текст теоретического материала, возвращаемый API системы обучения.
/// </summary>
public sealed record TheoryTextResponse(
    string Text,
    string Name);


