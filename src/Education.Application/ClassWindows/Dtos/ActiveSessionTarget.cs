using Education.Domain.PracticalModules;

namespace Education.Application.ClassWindows;

/// <summary>Идущая сессия внешнего модуля вместе с данными для повторного пуша.</summary>
public sealed record ActiveSessionTarget(
    PracticalModuleSession Session,
    Guid PracticalModuleId,
    string ExternalTaskRef,
    int? AttemptsPerSession,
    int? TimeLimitMinutes);
