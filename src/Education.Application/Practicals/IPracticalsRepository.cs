using Education.Application.Common;
using Education.Domain.Practicals;

namespace Education.Application.Practicals;

/// <summary>
/// Предоставляет операции чтения и записи практических материалов.
/// </summary>
public interface IPracticalsRepository : IBaseRepository<PracticalMaterial, long>
{
    /// <summary>
    /// Проверяет, принадлежит ли практический материал курсу указанного преподавателя.
    /// </summary>
    Task<bool> IsPracticalOwnerAsync(long practicalId, long teacherUserId, CancellationToken cancellationToken = default);

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
