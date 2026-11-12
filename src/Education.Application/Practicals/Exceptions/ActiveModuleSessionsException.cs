namespace Education.Application.Practicals;

/// <summary>
/// Сущность нельзя удалить: по её практикам идут активные сессии внешнего модуля.
/// Дождитесь завершения сессий или закройте пару.
/// </summary>
public sealed class ActiveModuleSessionsException(string entityName)
    : Exception($"Нельзя удалить {entityName}: по практикам идут активные сессии внешнего модуля. "
        + "Дождитесь их завершения или закройте пару.");
