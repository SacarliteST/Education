using System.Text.Json;

namespace Education.Contracts.PracticalModules;

/// <summary>Тело приёма оценки от модуля (заголовок <c>X-Service-Key</c> — вне тела).</summary>
public sealed record CompleteModuleSessionRequest(
    string SessionKey,
    int Grade,
    JsonElement? CompletionData,
    DateTimeOffset CompletedAt);
