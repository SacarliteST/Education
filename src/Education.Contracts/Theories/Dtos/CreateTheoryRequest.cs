namespace Education.Contracts.Theories;

/// <summary>
/// Запрос на создание теоретического материала внутри модуля.
/// </summary>
public sealed record CreateTheoryRequest(
    Guid ModuleId,
    string Name);


