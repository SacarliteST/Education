namespace Education.Application.TestResults;

/// <summary>
/// Состояние текущей попытки тестирования.
/// </summary>
public sealed record TestStatus(bool IsStarted, int? TryNumber);

