namespace Education.Application.PracticalModules;

/// <summary>У модуля не включён открытый банк заданий (<c>trainingBank.enabled</c> в конфигурации).</summary>
/// <param name="practicalModuleId">Идентификатор модуля.</param>
public sealed class TrainingBankDisabledException(Guid practicalModuleId)
    : InvalidOperationException($"У модуля '{practicalModuleId}' не включён банк заданий.")
{
    /// <summary>Идентификатор модуля.</summary>
    public Guid PracticalModuleId { get; } = practicalModuleId;
}
