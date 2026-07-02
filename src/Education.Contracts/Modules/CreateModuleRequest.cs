namespace Education.Contracts.Modules;

/// <summary>
/// Запрос на создание модуля внутри курса.
/// </summary>
public sealed record CreateModuleRequest(
    long CourseId,
    string Name);
