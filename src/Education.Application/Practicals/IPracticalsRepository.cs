using Education.Application.Common;
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
    /// Возвращает практические материалы модуля.
    /// </summary>
    Task<IReadOnlyList<PracticalMaterial>> GetPracticalsAsync(Guid moduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт практический материал.
    /// </summary>
    Task<PracticalMaterial> CreatePracticalAsync(CreatePracticalCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Публикует практический материал.
    /// </summary>
    Task PublishPracticalAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает задания практического материала.
    /// </summary>
    Task<IReadOnlyList<Case>> GetTasksAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает данные настройки вопросов практического материала.
    /// </summary>
    Task<PracticalQuestionsSetup?> GetQuestionsSetupAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Настраивает вопросы и пороги оценивания практического материала.
    /// </summary>
    Task ConfigureQuestionsAsync(ConfigurePracticalQuestionsCommand command, CancellationToken cancellationToken = default);
}

