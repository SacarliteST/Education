namespace Education.Application.ClassWindows;

/// <summary>
/// Временные границы пары, которыми ограничиваются результаты в ведомости и детализации.
/// </summary>
/// <param name="ClassWindowId">Идентификатор пары.</param>
/// <param name="StartsAt">Начало учитываемого интервала.</param>
/// <param name="EndsAt">Фактический конец с учётом досрочного завершения.</param>
public sealed record ClassWindowResultsScope(
    Guid ClassWindowId,
    DateTimeOffset StartsAt,
    DateTimeOffset EndsAt)
{
    /// <summary>
    /// Проверяет, началась ли попытка в интервале пары, включая обе границы.
    /// </summary>
    public bool Includes(DateTimeOffset startedAt) => startedAt >= StartsAt && startedAt <= EndsAt;
}
