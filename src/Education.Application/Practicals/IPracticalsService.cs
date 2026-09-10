using Education.Domain.Practicals;

namespace Education.Application.Practicals;

/// <summary>
/// Выполняет сценарии работы с практическими материалами.
/// </summary>
public interface IPracticalsService
{
    /// <summary>
    /// Возвращает практические материалы модуля.
    /// </summary>
    Task<IReadOnlyList<PracticalMaterial>> GetPracticalsAsync(Guid moduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает детали практики (вид, лимиты, привязку к модулю). <see langword="null"/> — практика не найдена.
    /// </summary>
    Task<PracticalDetail?> GetDetailAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт практический материал.
    /// </summary>
    Task<PracticalMaterial> CreatePracticalAsync(CreatePracticalCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Публикует практический материал.
    /// </summary>
    Task PublishPracticalAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет практический материал текущего преподавателя.
    /// </summary>
    Task DeletePracticalAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает задания практического материала.
    /// </summary>
    Task<IReadOnlyList<Case>> GetTasksAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает задание по идентификатору. <see langword="null"/> — не найдено.
    /// </summary>
    Task<Case?> GetTaskAsync(Guid taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт задание в практическом материале текущего преподавателя.
    /// </summary>
    Task<Case> CreateTaskAsync(CreateTaskCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет текст задания текущего преподавателя.
    /// </summary>
    Task UpdateTaskTextAsync(Guid taskId, UpdateTaskTextCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет задание текущего преподавателя.
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
    /// Привязывает практику текущего преподавателя к внешнему модулю (переводит в <c>kind=external</c>, 1:1).
    /// </summary>
    Task BindModuleAsync(BindPracticalModuleCommand command, CancellationToken cancellationToken = default);
}

