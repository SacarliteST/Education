using Education.Application.Common;
using Education.Application.PracticalModules;
using Education.Domain.Practicals;

namespace Education.Application.Practicals;

/// <summary>
/// Предоставляет операции чтения и записи практических материалов.
/// </summary>
public interface IPracticalsRepository : IBaseRepository<PracticalMaterial, Guid>
{
    /// <summary>
    /// Проверяет, принадлежит ли практический материал курсу указанного преподавателя.
    /// </summary>
    Task<bool> IsPracticalOwnerAsync(Guid practicalId, Guid teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, принадлежит ли задание курсу указанного преподавателя.
    /// </summary>
    Task<bool> IsTaskOwnerAsync(Guid taskId, Guid teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает практические материалы модуля.
    /// </summary>
    Task<IReadOnlyList<PracticalMaterial>> GetPracticalsAsync(Guid moduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает детали практики с привязкой к внешнему модулю. <see langword="null"/> — не найдена.
    /// </summary>
    Task<PracticalDetail?> GetDetailAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверяет, что задание принадлежит указанной практике.
    /// </summary>
    Task<bool> IsTaskInPracticalAsync(Guid taskId, Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Есть ли по практике работа студентов: загруженные файлы заданий или результаты теста.
    /// </summary>
    Task<bool> HasStudentActivityAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт практический материал.
    /// </summary>
    Task<PracticalMaterial> CreatePracticalAsync(CreatePracticalCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Публикует практический материал.
    /// </summary>
    Task PublishPracticalAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет практический материал по идентификатору.
    /// </summary>
    Task DeletePracticalAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает задания практического материала.
    /// </summary>
    Task<IReadOnlyList<Case>> GetTasksAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт задание практического материала.
    /// </summary>
    Task<Case> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет текст задания практического материала.
    /// </summary>
    Task UpdateTaskTextAsync(Guid taskId, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет задание практического материала по идентификатору.
    /// </summary>
    Task DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает данные настройки вопросов практического материала.
    /// </summary>
    Task<PracticalQuestionsSetup?> GetQuestionsSetupAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Настраивает вопросы и пороги оценивания практического материала.
    /// </summary>
    Task ConfigureQuestionsAsync(ConfigurePracticalQuestionsCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Привязывает практику к внешнему модулю: переводит в <c>kind=external</c>, задаёт лимиты,
    /// пересоздаёт единственный <see cref="Case"/> со ссылкой на модуль и ключ задания.
    /// </summary>
    Task BindModuleAsync(BindPracticalModuleCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Данные привязки внешнего задания практики: практика внешняя, задание принадлежит ей и модулю.
    /// </summary>
    /// <returns><see langword="null"/>, если условия не выполнены.</returns>
    Task<ExternalTaskBinding?> GetExternalTaskBindingAsync(
        Guid practicalId,
        Guid taskId,
        CancellationToken cancellationToken = default);
}

