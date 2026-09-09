namespace Education.Web.Integration;

/// <summary>Настройки подключения внешних модулей (секция <c>ModuleIntegration</c>).</summary>
internal sealed class ModuleIntegrationOptions
{
    public const string SectionKey = "ModuleIntegration";

    /// <summary>Зарегистрированный origin платформы для сборки return URL.</summary>
    public string PlatformOrigin { get; init; } = "http://localhost:5173";

    /// <summary>
    /// Origin веб-морды модуля для сборки launch URL. Пусто → берётся <see cref="PlatformOrigin"/>
    /// (единый origin за общим reverse-proxy). Задавать при безшлюзовой разработке, когда SPA
    /// модуля живёт на отдельном порту (напр. <c>http://localhost:5174</c>).
    /// </summary>
    public string ModuleWebOrigin { get; init; } = String.Empty;

    /// <summary>
    /// Шаблон возврата. Плейсхолдеры: <c>{origin}</c>, <c>{courseId}</c>, <c>{moduleId}</c>,
    /// <c>{practicalId}</c>, <c>{sessionId}</c>. По умолчанию — вложенный маршрут practical-страницы platform-web.
    /// </summary>
    public string ReturnUrlTemplate { get; init; } =
        "{origin}/student/courses/{courseId}/modules/{moduleId}/practicals/{practicalId}?session={sessionId}";

    /// <summary>URL эндпоинта Token Exchange IdentityService.</summary>
    public string TokenExchangeUrl { get; init; } = "http://localhost:5101/api/v1/auth/token/exchange";

    /// <summary>client_id доверенного клиента Education в IdentityService.</summary>
    public string IdentityClientId { get; init; } = "education-core";

    /// <summary>client_secret доверенного клиента Education в IdentityService.</summary>
    public string IdentityClientSecret { get; init; } = String.Empty;
}
