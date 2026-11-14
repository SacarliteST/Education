namespace Education.Application.Grades;

/// <summary>Попытка прохождения внутреннего теста.</summary>
public sealed record StudentTestAttemptResult(
    Guid TestResultId,
    int TryNumber,
    double? Score,
    double? MaxScore,
    DateTimeOffset StartedAt,
    DateTimeOffset? TurnedAt,
    bool IsCompleted);
