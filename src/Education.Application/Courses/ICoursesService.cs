using Education.Contracts.Courses;
using Education.Contracts.Modules;
using Education.Contracts.Theories;

namespace Education.Application.Courses;

/// <summary>
/// Выполняет сценарии работы с курсами, модулями и теоретическими материалами.
/// </summary>
public interface ICoursesService
{
    /// <summary>
    /// Возвращает курсы, принадлежащие текущему преподавателю.
    /// </summary>
    Task<IReadOnlyList<CourseResponse>> GetTeacherCoursesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт курс для текущего преподавателя.
    /// </summary>
    Task<CourseResponse> CreateCourseAsync(CreateCourseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет курс текущего преподавателя.
    /// </summary>
    Task DeleteCourseAsync(long courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает курсы, назначенные текущему студенту.
    /// </summary>
    Task<IReadOnlyList<CourseResponse>> GetStudentCoursesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает модули указанного курса.
    /// </summary>
    Task<IReadOnlyList<ModuleResponse>> GetModulesAsync(long courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт модуль в курсе текущего преподавателя.
    /// </summary>
    Task<ModuleResponse> CreateModuleAsync(CreateModuleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет модуль из курса текущего преподавателя.
    /// </summary>
    Task DeleteModuleAsync(long moduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает теоретические материалы указанного модуля.
    /// </summary>
    Task<IReadOnlyList<TheoryListItemResponse>> GetTheoriesAsync(long moduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает текст указанного теоретического материала.
    /// </summary>
    Task<TheoryTextResponse?> GetTheoryTextAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает документы указанного теоретического материала.
    /// </summary>
    Task<IReadOnlyList<TheoryDocumentResponse>> GetTheoryDocsAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает ссылки указанного теоретического материала.
    /// </summary>
    Task<IReadOnlyList<TheoryLinkResponse>> GetTheoryLinksAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт теоретический материал в модуле текущего преподавателя.
    /// </summary>
    Task<TheoryListItemResponse> CreateTheoryAsync(CreateTheoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет документ к теоретическому материалу текущего преподавателя.
    /// </summary>
    Task<TheoryDocumentResponse> CreateTheoryDocumentAsync(
        long theoryMaterialId,
        string description,
        TheoryDocumentFile file,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет заголовок теоретического материала текущего преподавателя.
    /// </summary>
    Task UpdateTheoryTitleAsync(long theoryId, UpdateTheoryTitleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет текст теоретического материала текущего преподавателя.
    /// </summary>
    Task UpdateTheoryTextAsync(long theoryId, UpdateTheoryTextRequest request, CancellationToken cancellationToken = default);

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
    Task<TheoryLinkResponse> CreateTheoryLinkAsync(CreateTheoryLinkRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет ссылку теоретического материала текущего преподавателя.
    /// </summary>
    Task DeleteTheoryLinkAsync(long linkId, CancellationToken cancellationToken = default);
}
