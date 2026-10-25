namespace Education.Contracts.Practicals;

/// <summary>Привязка внешней практики к заданию модуля.</summary>
public sealed record ExternalModuleBindingResponse(
    Guid PracticalModuleId,
    string PracticalModuleSlug,
    string PracticalModuleName,
    Guid TaskId,
    string ExternalTaskRef);
