namespace Education.Application.TestResults;

/// <summary>
/// Выполняет сценарии прохождения тестирования.
/// </summary>
public interface ITestResultsService
{
    /// <summary>
    /// Возвращает состояние текущей попытки тестирования.
    /// </summary>
    Task<TestStatus> GetStatusAsync(long practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Запускает или продолжает попытку тестирования.
    /// </summary>
    Task<int> StartTestAsync(long practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает вопросы текущей попытки тестирования.
    /// </summary>
    Task<TestQuestions?> GetQuestionsAsync(long practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Отправляет ответы и завершает попытку тестирования.
    /// </summary>
    Task<TestProtocolSummary> SubmitTestAsync(SubmitTestCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает протоколы практического материала для текущего студента.
    /// </summary>
    Task<IReadOnlyList<TestProtocolSummary>> GetStudentProtocolsAsync(long practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает протоколы практического материала для преподавателя.
    /// </summary>
    Task<IReadOnlyList<TestProtocolSummary>> GetTeacherProtocolsAsync(long practicalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает подробный протокол попытки тестирования.
    /// </summary>
    Task<TestProtocol?> GetProtocolAsync(long testResultId, CancellationToken cancellationToken = default);
}
