namespace Education.Contracts.Practicals;

/// <summary>Запрос привязки внешнего модуля к практике.</summary>
public sealed record BindPracticalModuleRequest(
    Guid PracticalModuleId,
    string ExternalTaskRef,
    int TriesCount,
    int? TimeLimitMinutes);
