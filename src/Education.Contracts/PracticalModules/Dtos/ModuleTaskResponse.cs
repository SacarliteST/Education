namespace Education.Contracts.PracticalModules;

/// <summary>Задание из каталога внешнего практического модуля.</summary>
/// <param name="Ref">Непрозрачный ключ задания внутри модуля.</param>
/// <param name="Name">Название задания.</param>
/// <param name="Description">Описание задания.</param>
/// <param name="DifficultyLevel">Уровень сложности по шкале модуля или <see langword="null"/>, если модуль его не отдал.</param>
/// <param name="TopicId">Идентификатор темы модуля или <see langword="null"/>, если модуль её не отдал.</param>
/// <param name="TopicName">Название темы модуля или <see langword="null"/>, если модуль её не отдал.</param>
public sealed record ModuleTaskResponse(
    string Ref,
    string Name,
    string Description,
    short? DifficultyLevel,
    string? TopicId,
    string? TopicName);
