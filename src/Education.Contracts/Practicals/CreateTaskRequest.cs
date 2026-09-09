namespace Education.Contracts.Practicals;

/// <summary>
/// Запрос на создание задания практического материала.
/// </summary>
public sealed record CreateTaskRequest(
    string Name);
