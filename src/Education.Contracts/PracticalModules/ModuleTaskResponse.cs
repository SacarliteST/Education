namespace Education.Contracts.PracticalModules;

/// <summary>Задание из каталога внешнего практического модуля.</summary>
public sealed record ModuleTaskResponse(string Ref, string Name, string Description);
