namespace Education.Contracts.PracticalModules;

/// <summary>Запрос на изменение записи реестра практического модуля. Slug не меняется.</summary>
public sealed record UpdatePracticalModuleRequest(
    string Name,
    string Description,
    string PracticeType,
    string BasePath,
    string IdentityAudience,
    string? Configuration,
    bool IsEnabled);
