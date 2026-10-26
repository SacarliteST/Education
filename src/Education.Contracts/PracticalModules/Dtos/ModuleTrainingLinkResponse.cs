namespace Education.Contracts.PracticalModules;

/// <summary>Ссылка входа студента в банк заданий модуля.</summary>
/// <param name="Url">Адрес перехода (токен только во фрагменте). Открывать в текущей вкладке.</param>
/// <param name="ExpiresInSeconds">Время жизни вложенного токена, секунды.</param>
public sealed record ModuleTrainingLinkResponse(string Url, int ExpiresInSeconds);
