using Education.Application.Common;
using Education.Domain.Materials;

namespace Education.Application.Theories;

/// <summary>
/// Предоставляет операции чтения и записи данных теоретических материалов.
/// </summary>
public interface ITheoriesRepository : IBaseRepository<TheoreticalMaterial, Guid>
{
    /// <summary>
    /// Проверяет, принадлежит ли теоретический материал указанному преподавателю.
    /// </summary>
    Task<bool> IsTheoryOwnerAsync(Guid theoryId, Guid teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли ссылка теоретического материала указанному преподавателю.
    /// </summary>
    Task<bool> IsTheoryLinkOwnerAsync(Guid linkId, Guid teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли документ теоретического материала указанному преподавателю.
    /// </summary>
    Task<bool> IsTheoryDocumentOwnerAsync(Guid documentId, Guid teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает теоретический материал.
    /// </summary>
    Task<TheoreticalMaterial?> GetTheoryAsync(Guid theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает документы теоретического материала.
    /// </summary>
    Task<IReadOnlyList<TheoreticalMaterialFile>> GetTheoryDocsAsync(Guid theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает ссылки теоретического материала.
    /// </summary>
    Task<IReadOnlyList<TheoreticalMaterialLink>> GetTheoryLinksAsync(Guid theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт теоретический материал.
    /// </summary>
    Task<TheoreticalMaterial> CreateTheoryAsync(CreateTheoryCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт запись документа теоретического материала.
    /// </summary>
    Task<TheoreticalMaterialFile> CreateTheoryDocumentAsync(
        Guid theoryMaterialId,
        string description,
        string path,
        string originalFileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет заголовок теоретического материала.
    /// </summary>
    Task UpdateTheoryTitleAsync(Guid theoryId, string title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет текст теоретического материала.
    /// </summary>
    Task UpdateTheoryTextAsync(Guid theoryId, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет теоретический материал.
    /// </summary>
    Task DeleteTheoryAsync(Guid theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает путь документа теоретического материала.
    /// </summary>
    Task<string?> GetTheoryDocumentPathAsync(Guid documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет запись документа теоретического материала.
    /// </summary>
    Task DeleteTheoryDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт ссылку теоретического материала.
    /// </summary>
    Task<TheoreticalMaterialLink> CreateTheoryLinkAsync(CreateTheoryLinkCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет ссылку теоретического материала.
    /// </summary>
    Task DeleteTheoryLinkAsync(Guid linkId, CancellationToken cancellationToken = default);
}

