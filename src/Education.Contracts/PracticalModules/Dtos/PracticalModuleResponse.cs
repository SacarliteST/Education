namespace Education.Contracts.PracticalModules;

/// <summary>Запись реестра внешнего практического модуля.</summary>
public sealed record PracticalModuleResponse(
    Guid Id,
    string Slug,
    string Name,
    string Description,
    string PracticeType,
    string BasePath,
    string IdentityAudience,
    bool IsEnabled,
    string Configuration);
