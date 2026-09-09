namespace Education.Application.PracticalModules;

/// <summary>Ручка каталога заданий модуля недоступна, вернула ошибку или неразборчивый ответ.</summary>
public sealed class ModuleCatalogUnavailableException(string message) : Exception(message);
