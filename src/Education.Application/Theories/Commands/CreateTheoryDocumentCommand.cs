namespace Education.Application.Theories;

/// <summary>
/// Команда добавления документа к теоретическому материалу.
/// </summary>
/// <param name="TheoryMaterialId">Идентификатор теоретического материала.</param>
/// <param name="Description">Описание документа.</param>
/// <param name="File">Загружаемый файл документа.</param>
public sealed record CreateTheoryDocumentCommand(long TheoryMaterialId, string Description, TheoryDocumentFile File);
