namespace Education.Application.PracticalModules;

public sealed record UpdatePracticalModuleCommand(
    string Name,
    string Description,
    string PracticeType,
    string BasePath,
    string IdentityAudience,
    string Configuration,
    bool IsEnabled);
