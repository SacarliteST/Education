using System.Text.Json;

namespace Education.Contracts.PracticalModules;

/// <summary>Запрос запуска/продолжения попытки внешнего модуля.</summary>
public sealed record StartModuleSessionRequest(Guid TaskId);

/// <summary>Ответ запуска: адрес перехода в модуль и метаданные попытки.</summary>
public sealed record StartModuleSessionResponse(
    Guid SessionId,
    string LaunchUrl,
    DateTimeOffset? ExpiresAt,
    int TryNumber,
    bool Resumed);

/// <summary>Статус попытки внешнего модуля.</summary>
public sealed record ModuleSessionResponse(
    Guid SessionId,
    string Status,
    int TryNumber,
    DateTimeOffset StartedAt,
    DateTimeOffset? ExpiresAt,
    string? EndReason,
    int? Grade,
    DateTimeOffset? EndedAt);

/// <summary>Данные гейта кнопок «Начать»/«Продолжить»/«Прервать».</summary>
public sealed record CurrentModuleSessionResponse(
    ModuleSessionResponse? Session,
    int AttemptsCount,
    int TriesCount,
    int? TimeLimitMinutes,
    int? BestGrade);

/// <summary>Тело приёма оценки от модуля (заголовок <c>X-Service-Key</c> — вне тела).</summary>
public sealed record CompleteModuleSessionRequest(
    string SessionKey,
    int Grade,
    JsonElement? CompletionData,
    DateTimeOffset CompletedAt);

/// <summary>Одно событие ленты «цифрового следа» попытки.</summary>
public sealed record ModuleSessionEventResponse(
    Guid EventId,
    string Kind,
    DateTimeOffset OccurredAt,
    JsonElement Payload);

/// <summary>Строка списка попыток внешнего модуля для преподавателя.</summary>
public sealed record ModuleSessionSummaryResponse(
    Guid SessionId,
    Guid UserId,
    string StudentName,
    int TryNumber,
    string Status,
    string? EndReason,
    int? Grade,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt);
