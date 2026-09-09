using Education.Domain.Common;

namespace Education.Domain.PracticalModules;

/// <summary>
/// Попытка прохождения внешнего практического модуля. Создаётся при запуске
/// (<see cref="ModuleSessionState.Active"/>), терминальна в
/// <see cref="ModuleSessionState.Completed"/> (пришла оценка по HTTP) или
/// <see cref="ModuleSessionState.Expired"/> (истекло время / прервана студентом).
/// </summary>
public sealed class PracticalModuleSession : Entity
{
    /// <summary>Жёсткий потолок жизни ACTIVE-сессии — сборка мусора при отсутствии лимита времени.</summary>
    public static readonly TimeSpan HardCeiling = TimeSpan.FromHours(24);

    /// <summary>Идентификатор задания (<c>Case.Id</c>) внешней практики.</summary>
    public Guid PracticalTaskId { get; private set; }

    /// <summary>Пользователь учебной системы (legacy id, не identity sub).</summary>
    public Guid UserId { get; private set; }

    /// <summary>Номер попытки, 1..triesCount.</summary>
    public int TryNumber { get; private set; }

    /// <summary>Состояние попытки.</summary>
    public ModuleSessionState Status { get; private set; } = ModuleSessionState.Active;

    /// <summary>Причина завершения для UI. <see langword="null"/>, пока попытка активна.</summary>
    public ModuleSessionEndReason? EndReason { get; private set; }

    /// <summary>Секрет сессии: подпись Kafka-событий и аутентификация HTTP-оценки. Наружу не отдаётся.</summary>
    public string SessionKey { get; private set; } = String.Empty;

    /// <summary>Канонический адрес возврата на платформу (собирает ядро).</summary>
    public string ReturnUrl { get; private set; } = String.Empty;

    /// <summary>Момент старта попытки.</summary>
    public DateTimeOffset StartedAt { get; private set; }

    /// <summary><c>StartedAt + timeLimitMinutes</c> или <see langword="null"/> (без лимита).</summary>
    public DateTimeOffset? ExpiresAt { get; private set; }

    /// <summary>Момент терминального перехода.</summary>
    public DateTimeOffset? EndedAt { get; private set; }

    /// <summary>Оценка 0..100, ставит модуль. Ядро не пересчитывает.</summary>
    public int? Grade { get; private set; }

    /// <summary>Произвольные метаданные завершения от модуля (jsonb).</summary>
    public string? CompletionData { get; private set; }

    private PracticalModuleSession()
    {
    }

    /// <param name="id">Заранее сгенерированный id — нужен, чтобы вложить его в <paramref name="returnUrl"/>.</param>
    public PracticalModuleSession(
        Guid id,
        Guid practicalTaskId,
        Guid userId,
        int tryNumber,
        string sessionKey,
        string returnUrl,
        DateTimeOffset startedAt,
        DateTimeOffset? expiresAt)
    {
        Id = id;
        PracticalTaskId = practicalTaskId;
        UserId = userId;
        TryNumber = tryNumber;
        SessionKey = sessionKey;
        ReturnUrl = returnUrl;
        StartedAt = startedAt;
        ExpiresAt = expiresAt;
        Status = ModuleSessionState.Active;
    }

    /// <summary><see langword="true"/>, если сессия ещё принимает работу студента.</summary>
    public bool IsActive => Status == ModuleSessionState.Active;

    /// <summary>Истекла ли ACTIVE-сессия по лимиту времени или потолку 24ч.</summary>
    public bool IsExpiredBy(DateTimeOffset now) =>
        Status == ModuleSessionState.Active
        && ((ExpiresAt is { } expiry && now >= expiry) || now >= StartedAt + HardCeiling);

    /// <summary>Терминальный переход в <see cref="ModuleSessionState.Expired"/>. Идемпотентен для уже терминальной сессии.</summary>
    public void Expire(DateTimeOffset now, ModuleSessionEndReason reason)
    {
        if (Status != ModuleSessionState.Active)
        {
            return;
        }

        Status = ModuleSessionState.Expired;
        EndReason = reason;
        EndedAt = now;
    }

    /// <summary>Терминальный переход в <see cref="ModuleSessionState.Completed"/> с оценкой модуля.</summary>
    public void Complete(DateTimeOffset now, int grade, string? completionData)
    {
        if (Status != ModuleSessionState.Active)
        {
            throw new InvalidOperationException($"Нельзя завершить сессию в состоянии '{Status}'.");
        }

        Status = ModuleSessionState.Completed;
        EndReason = ModuleSessionEndReason.Completed;
        EndedAt = now;
        Grade = grade;
        CompletionData = completionData;
    }
}
