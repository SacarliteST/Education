namespace Education.Contracts.PracticalModules;

/// <summary>Модуль с включённым открытым банком заданий.</summary>
/// <param name="Id">Идентификатор модуля.</param>
/// <param name="Slug">Короткий идентификатор модуля.</param>
/// <param name="Name">Название модуля.</param>
/// <param name="Description">Описание модуля.</param>
public sealed record TrainingModuleResponse(Guid Id, string Slug, string Name, string Description);
