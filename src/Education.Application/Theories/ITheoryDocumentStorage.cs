namespace Education.Application.Theories;

/// <summary>
/// Управляет физическим хранением документов теоретических материалов.
/// </summary>
public interface ITheoryDocumentStorage
{
    /// <summary>
    /// Сохраняет документ и возвращает путь, пригодный для записи в базу данных.
    /// </summary>
    Task<string> SaveAsync(TheoryDocumentFile file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет сохранённый документ по ранее записанному пути.
    /// </summary>
    Task DeleteAsync(string path, CancellationToken cancellationToken = default);
}
