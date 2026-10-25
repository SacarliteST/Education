namespace Education.Application.PracticalModules;

/// <summary>Итог приёма оценки от модуля.</summary>
public enum CompleteOutcome
{
    /// <summary>Сессия не найдена или секрет сессии не совпал.</summary>
    Unauthorized,
    /// <summary>Оценка принята.</summary>
    Accepted,
    /// <summary>Сессия уже завершена, повторная оценка проигнорирована.</summary>
    AlreadyCompleted,
    /// <summary>Сессия уже закрыта (истекла или прервана), оценка отброшена.</summary>
    SessionClosed,
}
