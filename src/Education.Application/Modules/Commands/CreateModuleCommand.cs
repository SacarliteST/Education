namespace Education.Application.Modules;

/// <summary>
/// Команда создания модуля курса.
/// </summary>
/// <param name="CourseId">Идентификатор курса.</param>
/// <param name="Name">Название модуля.</param>
public sealed record CreateModuleCommand(long CourseId, string Name);
