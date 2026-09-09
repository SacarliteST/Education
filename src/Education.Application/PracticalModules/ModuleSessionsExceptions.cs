namespace Education.Application.PracticalModules;

/// <summary>Лимит запусков внешней практики исчерпан (учитываются и <c>EXPIRED</c>).</summary>
public sealed class TriesExhaustedException() : Exception("Попытки исчерпаны.");

/// <summary>Практика не является внешней, либо задание не привязано к модулю.</summary>
public sealed class ExternalTaskNotFoundException() : Exception("Внешнее задание не найдено.");

/// <summary>Бэкенд модуля не принял пуш сессии.</summary>
public sealed class ModulePushFailedException(string message) : Exception(message);
