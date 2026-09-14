namespace Education.Application.Audit;

/// <summary>
/// Записывает административные действия в журнал Education (не путать с
/// аудитом IdentityService — тот про логины/роли/блокировки, этот — про
/// то, что происходит внутри Education).
/// </summary>
public interface IAdminEventRecorder
{
    /// <summary>
    /// Записывает событие. Инициатор берётся из текущего HTTP-контекста
    /// автоматически — вызывающему коду достаточно указать тип и описание.
    /// </summary>
    Task RecordAsync(string eventType, string description, CancellationToken cancellationToken = default);
}
