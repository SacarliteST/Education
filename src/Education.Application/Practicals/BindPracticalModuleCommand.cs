namespace Education.Application.Practicals;

/// <summary>Команда привязки внешнего модуля к практике (1:1).</summary>
public sealed record BindPracticalModuleCommand(
    Guid PracticalId,
    Guid PracticalModuleId,
    string ExternalTaskRef,
    int TriesCount,
    int? TimeLimitMinutes);
