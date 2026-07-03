namespace Education.Contracts.TestResults;

/// <summary>
/// Краткие данные протокола попытки тестирования.
/// </summary>
public sealed record TestProtocolSummaryResponse(
    Guid Id,
    Guid UserId,
    double? Score,
    double? MaxScore,
    int TryNumber,
    int Grade);


