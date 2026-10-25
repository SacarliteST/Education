namespace Education.Application.PracticalModules;

/// <summary>Бэкенд модуля не принял пуш сессии.</summary>
public sealed class ModulePushFailedException(string message) : Exception(message);
