namespace Education.Application.PracticalModules;

/// <summary>Готовая ссылка входа студента в банк заданий модуля.</summary>
/// <param name="Url">Адрес перехода; токен лежит только во фрагменте.</param>
/// <param name="ExpiresInSeconds">Время жизни вложенного токена, секунды.</param>
public sealed record ModuleTrainingLink(string Url, int ExpiresInSeconds);
