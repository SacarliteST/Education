using Education.Application.Common;
using Education.Domain.Tests;

namespace Education.Application.Questions;

/// <summary>
/// Предоставляет операции чтения и записи вопросов тестирования.
/// </summary>
public interface IQuestionsRepository : IBaseRepository<Question, long>
{
    /// <summary>
    /// Проверяет, принадлежит ли вопрос курсу указанного преподавателя.
    /// </summary>
    Task<bool> IsQuestionOwnerAsync(long questionId, long teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает вопросы модуля.
    /// </summary>
    Task<IReadOnlyList<Question>> GetQuestionsAsync(long moduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт вопрос.
    /// </summary>
    Task<Question> CreateQuestionAsync(CreateQuestionCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет вопрос.
    /// </summary>
    Task UpdateQuestionAsync(long questionId, UpdateQuestionCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет вопрос.
    /// </summary>
    Task DeleteQuestionAsync(long questionId, CancellationToken cancellationToken = default);
}
