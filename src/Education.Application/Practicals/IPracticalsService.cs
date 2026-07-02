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
    Task<IReadOnlyList<PracticalMaterial>> GetPracticalsAsync(long moduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт практический материал.
    /// </summary>
    Task<PracticalMaterial> CreatePracticalAsync(CreatePracticalCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Публикует практический материал.
    /// </summary>
    Task PublishPracticalAsync(long practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает задания практического материала.
    /// </summary>
    Task<IReadOnlyList<Case>> GetTasksAsync(long practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает данные настройки вопросов практического материала.
    /// </summary>
    Task<PracticalQuestionsSetup?> GetQuestionsSetupAsync(long practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Настраивает вопросы и пороги оценивания практического материала.
    /// </summary>
    Task ConfigureQuestionsAsync(ConfigurePracticalQuestionsCommand command, CancellationToken cancellationToken = default);
}
