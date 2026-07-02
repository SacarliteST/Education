namespace Education.Contracts.Theories;

/// <summary>
/// Краткие данные теоретического материала для списков внутри модуля.
/// </summary>
public sealed record TheoryListItemResponse(
    long Id,
    string Name);
