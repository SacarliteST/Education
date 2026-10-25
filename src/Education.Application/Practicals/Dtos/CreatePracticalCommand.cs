namespace Education.Application.Practicals;

/// <summary>
/// Команда создания практического материала.
/// </summary>
public sealed record CreatePracticalCommand(Guid ModuleId, string Name);

