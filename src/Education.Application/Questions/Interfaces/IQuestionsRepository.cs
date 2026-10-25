using Education.Application.Common;
using Education.Domain.Tests;

namespace Education.Application.Questions;

/// <summary>
/// Предоставляет операции чтения и записи вопросов тестирования.
/// </summary>
public interface IQuestionsRepository : IBaseRepository<Question, Guid>
{
    /// <summary>
    /// Проверяет, принадлежит ли вопрос курсу указанного преподавателя.
    /// </summary>
    Task<bool> IsQuestionOwnerAsync(Guid questionId, Guid teacherUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает вопросы модуля.
    /// </summary>
    Task<IReadOnlyList<Question>> GetQuestionsAsync(Guid moduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт вопрос.
    /// </summary>
    Task<Question> CreateQuestionAsync(CreateQuestionCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет вопрос.
    /// </summary>
    Task UpdateQuestionAsync(Guid questionId, UpdateQuestionCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет вопрос.
    /// </summary>
    Task DeleteQuestionAsync(Guid questionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает сумму весов вопросов с указанными идентификаторами.
    /// </summary>
    Task<double> SumWeightsAsync(IReadOnlyCollection<Guid> questionIds, CancellationToken cancellationToken = default);
}

