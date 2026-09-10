namespace Education.Application.PracticalModules;

/// <summary>Сборка URL запуска и возврата из зарегистрированного origin платформы (конфиг Web).</summary>
public interface IModuleIntegrationConfig
{
    /// <summary>Канонический адрес возврата на вложенную страницу практики студента.</summary>
    string BuildReturnUrl(Guid courseId, Guid moduleId, Guid practicalId, Guid sessionId);

    /// <summary><c>{origin}{basePath}/launch?session={id}#access_token={token}</c>.</summary>
    string BuildLaunchUrl(string moduleBasePath, Guid sessionId, string accessToken);

    /// <summary>
    /// <c>{origin}{basePath}/teacher/launch#access_token={token}</c> — SSO преподавателя
    /// в контур авторинга модуля. Без сессии и <c>?session=</c>.
    /// </summary>
    string BuildAuthoringUrl(string moduleBasePath, string accessToken);
}
