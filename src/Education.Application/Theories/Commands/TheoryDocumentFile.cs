namespace Education.Application.Theories;

/// <summary>
/// Файл документа теоретического материала.
/// </summary>
/// <param name="FileName">Исходное имя файла.</param>
/// <param name="Content">Содержимое файла.</param>
public sealed record TheoryDocumentFile(string FileName, Stream Content);
