namespace Education.Application.Practicals;

/// <summary>Привязка внешней практики к заданию модуля.</summary>
public sealed record ExternalModuleBinding(
    Guid PracticalModuleId,
    string PracticalModuleSlug,
    string PracticalModuleName,
    Guid TaskId,
    string ExternalTaskRef);
