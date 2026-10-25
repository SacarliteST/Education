namespace Education.Contracts.TestResults;

/// <summary>
/// Состояние текущей попытки тестирования.
/// </summary>
public sealed record TestStatusResponse(
    bool IsStarted,
    int? TryNumber);


