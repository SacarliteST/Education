namespace Education.Application.PracticalModules;

/// <summary>Сборка URL запуска и возврата из зарегистрированного origin платформы (конфиг Web).</summary>
public interface IModuleIntegrationConfig
{
    /// <summary>Канонический адрес возврата на вложенную страницу практики студента.</summary>
    string BuildReturnUrl(Guid courseId, Guid moduleId, Guid practicalId, Guid sessionId);

    /// <summary><c>{origin}{basePath}/launch?session={id}#access_token={token}</c>.</summary>
    string BuildLaunchUrl(string moduleBasePath, Guid sessionId, string accessToken);

    /// <summary>
    /// <c>{origin}{basePath}/teacher/launch[?return={path}&amp;task={ref}]#access_token={token}</c> —
    /// SSO преподавателя в контур авторинга модуля. Без сессии и <c>?session=</c>.
    /// Токен только во фрагменте; <c>return</c> и <c>task</c> — в query и необязательны.
    /// </summary>
    /// <param name="moduleBasePath">Базовый путь модуля за общим reverse-proxy.</param>
    /// <param name="accessToken">Токен под audience модуля.</param>
    /// <param name="returnPath">Проверенный относительный путь возврата на платформу или <see langword="null"/>.</param>
    /// <param name="taskRef">Проверенная ссылка на задание модуля или <see langword="null"/>.</param>
    string BuildAuthoringUrl(string moduleBasePath, string accessToken, string? returnPath = null, string? taskRef = null);
}
