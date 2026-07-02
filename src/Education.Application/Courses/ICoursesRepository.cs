using Education.Contracts.Courses;
using Education.Contracts.Modules;
using Education.Contracts.Theories;

namespace Education.Application.Courses;

/// <summary>
/// Предоставляет операции чтения и записи данных курсов для сценариев приложения.
/// </summary>
public interface ICoursesRepository
{
    /// <summary>
    /// Возвращает курсы указанного преподавателя.
    /// </summary>
    Task<IReadOnlyList<CourseResponse>> GetTeacherCoursesAsync(long teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт курс для указанного преподавателя.
    /// </summary>
    Task<CourseResponse> CreateCourseAsync(long teacherUserId, CreateCourseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет курс по идентификатору.
    /// </summary>
    Task DeleteCourseAsync(long courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает курсы, назначенные указанному студенту.
    /// </summary>
    Task<IReadOnlyList<CourseResponse>> GetStudentCoursesAsync(long studentUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли курс указанному преподавателю.
    /// </summary>
    Task<bool> IsCourseOwnerAsync(long courseId, long teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли модуль курсу указанного преподавателя.
    /// </summary>
    Task<bool> IsModuleOwnerAsync(long moduleId, long teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли теоретический материал указанному преподавателю.
    /// </summary>
    Task<bool> IsTheoryOwnerAsync(long theoryId, long teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли ссылка теоретического материала указанному преподавателю.
    /// </summary>
    Task<bool> IsTheoryLinkOwnerAsync(long linkId, long teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает модули указанного курса.
    /// </summary>
    Task<IReadOnlyList<ModuleResponse>> GetModulesAsync(long courseId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт модуль.
    /// </summary>
    Task<ModuleResponse> CreateModuleAsync(CreateModuleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет модуль по идентификатору.
    /// </summary>
    Task DeleteModuleAsync(long moduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список теоретических материалов указанного модуля.
    /// </summary>
    Task<IReadOnlyList<TheoryListItemResponse>> GetTheoriesAsync(long moduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает текст теоретического материала.
    /// </summary>
    Task<TheoryTextResponse?> GetTheoryTextAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает документы теоретического материала.
    /// </summary>
    Task<IReadOnlyList<TheoryDocumentResponse>> GetTheoryDocsAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает ссылки теоретического материала.
    /// </summary>
    Task<IReadOnlyList<TheoryLinkResponse>> GetTheoryLinksAsync(long theoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт теоретический материал.
    /// </summary>
    Task<TheoryListItemResponse> CreateTheoryAsync(CreateTheoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт запись документа теоретического материала.
    /// </summary>
    Task<TheoryDocumentResponse> CreateTheoryDocumentAsync(
        long theoryMaterialId,
        string description,
        string path,
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
    /// Проверяет, принадлежит ли документ теоретического материала указанному преподавателю.
    /// </summary>
    Task<bool> IsTheoryDocumentOwnerAsync(long documentId, long teacherUserId, CancellationToken cancellationToken = default);

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
    Task<TheoryLinkResponse> CreateTheoryLinkAsync(CreateTheoryLinkRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет ссылку теоретического материала.
    /// </summary>
    Task DeleteTheoryLinkAsync(long linkId, CancellationToken cancellationToken = default);
}
