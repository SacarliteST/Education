namespace Education.Contracts.PracticalModules;

/// <summary>Запрос на регистрацию нового практического модуля.</summary>
public sealed record CreatePracticalModuleRequest(
    string Slug,
    string Name,
    string Description,
    string PracticeType,
    string BasePath,
    string IdentityAudience,
    string? Configuration);
