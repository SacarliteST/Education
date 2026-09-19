namespace Education.Contracts.PracticalModules;

/// <summary>
/// Ссылка для SSO-перехода преподавателя в контур авторинга внешнего модуля.
/// <see cref="Url"/> открывается в текущей вкладке; <see cref="ExpiresInSeconds"/> —
/// сколько ещё живёт вложенный в него токен.
/// </summary>
public sealed record ModuleAuthoringLinkResponse(string Url, int ExpiresInSeconds);
