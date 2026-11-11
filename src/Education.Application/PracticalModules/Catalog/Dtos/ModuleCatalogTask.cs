namespace Education.Application.PracticalModules;

/// <summary>Задание из каталога внешнего практического модуля (нормализованное ядром).</summary>
/// <param name="Ref">Непрозрачный ключ задания внутри модуля.</param>
/// <param name="Name">Название задания.</param>
/// <param name="Description">Описание задания.</param>
public sealed record ModuleCatalogTask(string Ref, string Name, string Description)
{
    /// <summary>Уровень сложности по шкале модуля; <see langword="null"/>, если модуль его не отдал.</summary>
    public short? DifficultyLevel { get; init; }

    /// <summary>Идентификатор темы модуля; <see langword="null"/>, если модуль её не отдал.</summary>
    public string? TopicId { get; init; }

    /// <summary>Название темы модуля; <see langword="null"/>, если модуль её не отдал.</summary>
    public string? TopicName { get; init; }
}
