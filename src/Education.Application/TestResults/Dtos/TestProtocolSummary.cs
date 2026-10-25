namespace Education.Application.TestResults;

/// <summary>
/// Краткие данные протокола попытки тестирования.
/// </summary>
public sealed record TestProtocolSummary(
    Guid Id,
    Guid UserId,
    double? Score,
    double? MaxScore,
    int TryNumber,
    int Grade,
    string? StudentName = null,
    DateTimeOffset? StartedAt = null,
    DateTimeOffset? SubmittedAt = null,
    int? DurationSeconds = null);

