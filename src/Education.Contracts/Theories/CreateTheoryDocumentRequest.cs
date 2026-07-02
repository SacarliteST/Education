namespace Education.Contracts.Theories;

/// <summary>
/// Метаданные запроса на загрузку документа теоретического материала.
/// </summary>
public sealed record CreateTheoryDocumentRequest(
    long TheoryMaterialId,
    string Description);
