namespace Education.Application.PracticalModules;

/// <summary>Результат запуска/продолжения попытки внешнего модуля.</summary>
public sealed record StartModuleSessionResult(
    Guid SessionId,
    string LaunchUrl,
    DateTimeOffset? ExpiresAt,
    int TryNumber,
    bool Resumed);
