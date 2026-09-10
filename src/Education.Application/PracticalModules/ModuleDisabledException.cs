namespace Education.Application.PracticalModules;

/// <summary>Модуль зарегистрирован, но выключен (<c>IsEnabled == false</c>).</summary>
public sealed class ModuleDisabledException(Guid practicalModuleId)
    : Exception($"Практический модуль {practicalModuleId} выключен.");
