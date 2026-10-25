namespace Education.Application.TestResults;

/// <summary>
/// Подробные данные протокола попытки тестирования.
/// </summary>
public sealed record TestProtocol(
    IReadOnlyList<TestProtocolAnswer> Answers,
    int TryNumber,
    double? Score,
    double? MaxScore,
    int Grade);

