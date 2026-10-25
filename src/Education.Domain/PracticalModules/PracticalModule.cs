using Education.Domain.Common;

namespace Education.Domain.PracticalModules;

/// <summary>
/// Запись реестра внешнего практического модуля (Host-Plugin).
/// </summary>
public sealed class PracticalModule : Entity
{
    /// <summary>Короткий уникальный идентификатор модуля; часть URL проксируемого пути.</summary>
    public string Slug { get; private set; } = String.Empty;
    /// <summary>Название модуля.</summary>
    public string Name { get; private set; } = String.Empty;
    /// <summary>Описание модуля.</summary>
    public string Description { get; private set; } = String.Empty;
    /// <summary>Тип практики, которую обслуживает модуль.</summary>
    public string PracticeType { get; private set; } = String.Empty;
    /// <summary>Базовый путь интерфейса модуля на платформе.</summary>
    public string BasePath { get; private set; } = String.Empty;
    /// <summary>Audience токена обмена для обращения к модулю.</summary>
    public string IdentityAudience { get; private set; } = String.Empty;
    /// <summary>Модуль включён и доступен для запуска практик.</summary>
    public bool IsEnabled { get; private set; }
    /// <summary>Конфигурация модуля в формате JSON.</summary>
    public string Configuration { get; private set; } = "{}";

    private PracticalModule()
    {
    }

    /// <summary>Регистрирует модуль; новый модуль включён.</summary>
    /// <param name="slug">Короткий уникальный идентификатор.</param>
    /// <param name="name">Название.</param>
    /// <param name="description">Описание.</param>
    /// <param name="practiceType">Тип практики.</param>
    /// <param name="basePath">Базовый путь интерфейса.</param>
    /// <param name="identityAudience">Audience токена обмена.</param>
    /// <param name="configuration">Конфигурация в формате JSON.</param>
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
    /// <param name="name">Название.</param>
    /// <param name="description">Описание.</param>
    /// <param name="practiceType">Тип практики.</param>
    /// <param name="basePath">Базовый путь интерфейса.</param>
    /// <param name="identityAudience">Audience токена обмена.</param>
    /// <param name="configuration">Конфигурация в формате JSON.</param>
    /// <param name="isEnabled">Модуль включён.</param>
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
