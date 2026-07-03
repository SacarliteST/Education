namespace Education.Contracts.Practicals;

/// <summary>
/// Запрос на создание практического материала внутри модуля.
/// </summary>
public sealed record CreatePracticalRequest(
    Guid ModuleId,
    string Name);


