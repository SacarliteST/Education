namespace Education.Contracts.TestResults;

/// <summary>
/// Подробные данные протокола попытки тестирования.
/// </summary>
public sealed record TestProtocolResponse(
    IReadOnlyList<TestProtocolAnswerResponse> Answers,
    int TryNumber,
    double? Score,
    double? MaxScore,
    int Grade);
