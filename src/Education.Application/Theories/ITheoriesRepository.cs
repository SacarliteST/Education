using Education.Application.Common;
using Education.Domain.Materials;

namespace Education.Application.Theories;

/// <summary>
/// Предоставляет операции чтения и записи данных теоретических материалов.
/// </summary>
public interface ITheoriesRepository : IBaseRepository<TheoreticalMaterial, long>
{
    /// <summary>
    /// Проверяет, принадлежит ли теоретический материал указанному преподавателю.
    /// </summary>
    Task<bool> IsTheoryOwnerAsync(long theoryId, long teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли ссылка теоретического материала указанному преподавателю.
    /// </summary>
    Task<bool> IsTheoryLinkOwnerAsync(long linkId, long teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли документ теоретического материала указанному преподавателю.
    /// </summary>
    Task<bool> IsTheoryDocumentOwnerAsync(long documentId, long teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает теоретический материал.
    /// </summary>
    Task<TheoreticalMaterial?> GetTheoryAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает документы теоретического материала.
    /// </summary>
    Task<IReadOnlyList<TheoreticalMaterialFile>> GetTheoryDocsAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает ссылки теоретического материала.
    /// </summary>
    Task<IReadOnlyList<TheoreticalMaterialLink>> GetTheoryLinksAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт теоретический материал.
    /// </summary>
    Task<TheoreticalMaterial> CreateTheoryAsync(CreateTheoryCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт запись документа теоретического материала.
    /// </summary>
    Task<TheoreticalMaterialFile> CreateTheoryDocumentAsync(
        long theoryMaterialId,
        string description,
        string path,
        string originalFileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет заголовок теоретического материала.
    /// </summary>
    Task UpdateTheoryTitleAsync(long theoryId, string title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет текст теоретического материала.
    /// </summary>
    Task UpdateTheoryTextAsync(long theoryId, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет теоретический материал.
    /// </summary>
    Task DeleteTheoryAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает путь документа теоретического материала.
    /// </summary>
    Task<string?> GetTheoryDocumentPathAsync(long documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет запись документа теоретического материала.
    /// </summary>
    Task DeleteTheoryDocumentAsync(long documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт ссылку теоретического материала.
    /// </summary>
    Task<TheoreticalMaterialLink> CreateTheoryLinkAsync(CreateTheoryLinkCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет ссылку теоретического материала.
    /// </summary>
    Task DeleteTheoryLinkAsync(long linkId, CancellationToken cancellationToken = default);
}
