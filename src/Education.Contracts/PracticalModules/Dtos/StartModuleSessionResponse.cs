using System.Text.Json;

namespace Education.Contracts.PracticalModules;

/// <summary>Ответ запуска: адрес перехода в модуль и метаданные попытки.</summary>
public sealed record StartModuleSessionResponse(
    Guid SessionId,
    string LaunchUrl,
    DateTimeOffset? ExpiresAt,
    int TryNumber,
    bool Resumed);
