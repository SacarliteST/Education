using Education.Domain.Materials;

namespace Education.Application.Theories;

/// <summary>
/// Выполняет сценарии работы с теоретическими материалами.
/// </summary>
public interface ITheoriesService
{
    /// <summary>
    /// Возвращает указанный теоретический материал.
    /// </summary>
    Task<TheoreticalMaterial?> GetTheoryAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает документы указанного теоретического материала.
    /// </summary>
    Task<IReadOnlyList<TheoreticalMaterialFile>> GetTheoryDocsAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает ссылки указанного теоретического материала.
    /// </summary>
    Task<IReadOnlyList<TheoreticalMaterialLink>> GetTheoryLinksAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт теоретический материал в модуле текущего преподавателя.
    /// </summary>
    Task<TheoreticalMaterial> CreateTheoryAsync(CreateTheoryCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет документ к теоретическому материалу текущего преподавателя.
    /// </summary>
    Task<TheoreticalMaterialFile> CreateTheoryDocumentAsync(
        CreateTheoryDocumentCommand command,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет заголовок теоретического материала текущего преподавателя.
    /// </summary>
    Task UpdateTheoryTitleAsync(long theoryId, UpdateTheoryTitleCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет текст теоретического материала текущего преподавателя.
    /// </summary>
    Task UpdateTheoryTextAsync(long theoryId, UpdateTheoryTextCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет теоретический материал текущего преподавателя.
    /// </summary>
    Task DeleteTheoryAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет документ теоретического материала текущего преподавателя.
    /// </summary>
    Task DeleteTheoryDocumentAsync(long documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет ссылку к теоретическому материалу текущего преподавателя.
    /// </summary>
    Task<TheoreticalMaterialLink> CreateTheoryLinkAsync(CreateTheoryLinkCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет ссылку теоретического материала текущего преподавателя.
    /// </summary>
    Task DeleteTheoryLinkAsync(long linkId, CancellationToken cancellationToken = default);
}
