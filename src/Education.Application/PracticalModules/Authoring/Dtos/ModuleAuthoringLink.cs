namespace Education.Application.PracticalModules;

/// <summary>
/// Готовая ссылка SSO-перехода преподавателя в контур авторинга модуля и остаток
/// жизни вложенного в неё токена (секунды).
/// </summary>
public sealed record ModuleAuthoringLink(string Url, int ExpiresInSeconds);
