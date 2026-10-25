namespace Education.Application.PracticalModules;

/// <summary>Модуль с указанным идентификатором не зарегистрирован или отключён.</summary>
public sealed class PracticalModuleNotFoundException(Guid practicalModuleId)
    : Exception($"Практический модуль {practicalModuleId} не найден или отключён.");
