namespace Education.Application.TestResults;

/// <summary>
/// Предоставляет операции чтения и записи результатов тестирования.
/// </summary>
public interface ITestResultsRepository
{
    /// <summary>
    /// Проверяет, назначен ли практический материал студенту.
    /// </summary>
    Task<bool> IsPracticalAssignedToStudentAsync(Guid practicalId, Guid studentUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает состояние текущей попытки тестирования.
    /// </summary>
    Task<TestStatus> GetStatusAsync(Guid practicalId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запускает или продолжает попытку тестирования.
    /// </summary>
    Task<int> StartTestAsync(Guid practicalId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает вопросы текущей попытки тестирования.
    /// </summary>
    Task<TestQuestions?> GetQuestionsAsync(Guid practicalId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправляет ответы и завершает попытку тестирования.
    /// </summary>
    Task<TestProtocolSummary> SubmitTestAsync(SubmitTestCommand command, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает протоколы студента по практическому материалу.
    /// </summary>
    Task<IReadOnlyList<TestProtocolSummary>> GetStudentProtocolsAsync(Guid practicalId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает протоколы практического материала.
    /// </summary>
    Task<IReadOnlyList<TestProtocolSummary>> GetTeacherProtocolsAsync(Guid practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает подробный протокол попытки тестирования.
    /// </summary>
    Task<TestProtocol?> GetProtocolAsync(Guid testResultId, CancellationToken cancellationToken = default);
}

