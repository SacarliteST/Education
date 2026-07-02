using Education.Domain.Tests;

namespace Education.Application.Questions;

/// <summary>
/// Выполняет сценарии работы с вопросами тестирования.
/// </summary>
public interface IQuestionsService
{
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
