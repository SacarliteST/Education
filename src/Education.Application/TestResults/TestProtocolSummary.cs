namespace Education.Application.TestResults;

/// <summary>
/// Краткие данные протокола попытки тестирования.
/// </summary>
public sealed record TestProtocolSummary(long Id, long UserId, double? Score, double? MaxScore, int TryNumber, int Grade);
