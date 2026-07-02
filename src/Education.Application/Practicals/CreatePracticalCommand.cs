namespace Education.Application.Practicals;

/// <summary>
/// Команда создания практического материала.
/// </summary>
public sealed record CreatePracticalCommand(long ModuleId, string Name);
