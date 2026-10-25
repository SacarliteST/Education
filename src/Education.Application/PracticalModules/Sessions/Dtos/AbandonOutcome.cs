namespace Education.Application.PracticalModules;

/// <summary>Итог попытки прервать сессию.</summary>
public enum AbandonOutcome
{
    /// <summary>Сессия не найдена или принадлежит другому пользователю.</summary>
    NotFound,
    /// <summary>Сессия уже завершена или истекла.</summary>
    AlreadyTerminal,
    /// <summary>Сессия прервана.</summary>
    Abandoned,
}
