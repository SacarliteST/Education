namespace Education.Application.PracticalModules;

/// <summary>Модуль, в котором включён открытый банк заданий для самостоятельной тренировки студента.</summary>
/// <param name="Id">Идентификатор модуля в реестре.</param>
/// <param name="Slug">Короткий идентификатор модуля.</param>
/// <param name="Name">Название модуля.</param>
/// <param name="Description">Описание модуля.</param>
public sealed record TrainingModule(Guid Id, string Slug, string Name, string Description);
