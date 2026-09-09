namespace Education.Application.Practicals;

/// <summary>
/// Команда создания задания практического материала.
/// </summary>
public sealed record CreateTaskCommand(Guid PracticalId, string Name);
