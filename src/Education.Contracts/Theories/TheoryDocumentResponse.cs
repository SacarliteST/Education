namespace Education.Contracts.Theories;

/// <summary>
/// Данные документа теоретического материала, возвращаемые API системы обучения.
/// </summary>
public sealed record TheoryDocumentResponse(
    long Id,
    string Path,
    string Description,
    string Name);
