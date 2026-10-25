namespace Education.Application.Theories;

/// <summary>
/// Команда создания теоретического материала.
/// </summary>
/// <param name="ModuleId">Идентификатор модуля.</param>
/// <param name="Name">Название теоретического материала.</param>
public sealed record CreateTheoryCommand(Guid ModuleId, string Name);

