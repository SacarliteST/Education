namespace Education.Application.PracticalModules;

/// <summary>Итог попытки прервать сессию.</summary>
public enum AbandonOutcome
{
    NotFound,
    AlreadyTerminal,
    Abandoned,
}
