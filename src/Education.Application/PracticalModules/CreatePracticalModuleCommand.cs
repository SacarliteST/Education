namespace Education.Application.PracticalModules;

public sealed record CreatePracticalModuleCommand(
    string Slug,
    string Name,
    string Description,
    string PracticeType,
    string BasePath,
    string IdentityAudience,
    string Configuration);
