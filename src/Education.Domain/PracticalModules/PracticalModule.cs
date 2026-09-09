using Education.Domain.Common;

namespace Education.Domain.PracticalModules;

/// <summary>
/// Запись реестра внешнего практического модуля (Host-Plugin).
/// </summary>
public sealed class PracticalModule : Entity
{
    public string Slug { get; private set; } = String.Empty;
    public string Name { get; private set; } = String.Empty;
    public string Description { get; private set; } = String.Empty;
    public string PracticeType { get; private set; } = String.Empty;
    public string BasePath { get; private set; } = String.Empty;
    public string IdentityAudience { get; private set; } = String.Empty;
    public bool IsEnabled { get; private set; }
    public string Configuration { get; private set; } = "{}";

    private PracticalModule()
    {
    }

    public PracticalModule(
        string slug,
        string name,
        string description,
        string practiceType,
        string basePath,
        string identityAudience,
        string configuration)
    {
        Slug = slug;
        Name = name;
        Description = description;
        PracticeType = practiceType;
        BasePath = basePath;
        IdentityAudience = identityAudience;
        Configuration = configuration;
        IsEnabled = true;
    }

    /// <summary>Slug не меняется — он часть URL проксируемого пути модуля.</summary>
    public void Update(
        string name,
        string description,
        string practiceType,
        string basePath,
        string identityAudience,
        string configuration,
        bool isEnabled)
    {
        Name = name;
        Description = description;
        PracticeType = practiceType;
        BasePath = basePath;
        IdentityAudience = identityAudience;
        Configuration = configuration;
        IsEnabled = isEnabled;
    }
}
