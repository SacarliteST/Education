namespace Education.Contracts.Modules;

/// <summary>
/// Данные модуля, возвращаемые API системы обучения.
/// </summary>
public sealed record ModuleResponse(
    Guid Id,
    string Name);


