namespace Education.Contracts.Grades;

/// <summary>
/// Попытка прохождения внутреннего теста.
/// </summary>
/// <param name="TestResultId">Идентификатор попытки.</param>
/// <param name="TryNumber">Порядковый номер попытки.</param>
/// <param name="Score">Набранные баллы или <see langword="null"/> для незавершённой попытки.</param>
/// <param name="MaxScore">Максимальные баллы или <see langword="null"/>, пока попытка не завершена.</param>
/// <param name="StartedAt">Момент начала попытки.</param>
/// <param name="TurnedAt">Момент сдачи или <see langword="null"/> для незавершённой попытки.</param>
/// <param name="IsCompleted">Признак завершённой попытки.</param>
public sealed record StudentTestAttemptResultResponse(
    Guid TestResultId,
    int TryNumber,
    double? Score,
    double? MaxScore,
    DateTimeOffset StartedAt,
    DateTimeOffset? TurnedAt,
    bool IsCompleted);
