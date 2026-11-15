namespace Education.Contracts.Grades;

/// <summary>
/// Сессия прохождения задания внешнего практического модуля.
/// </summary>
/// <param name="SessionId">Идентификатор сессии, используемый для открытия протокола событий.</param>
/// <param name="Status"><c>ACTIVE</c>, <c>COMPLETED</c> или <c>EXPIRED</c>.</param>
/// <param name="EndReason">Причина завершения или <see langword="null"/> для активной сессии.</param>
/// <param name="Grade">Баллы модуля от 0 до 100 или <see langword="null"/>, если результат не получен.</param>
/// <param name="StartedAt">Момент начала сессии.</param>
/// <param name="EndedAt">Момент завершения или <see langword="null"/> для активной сессии.</param>
public sealed record StudentPracticalSessionResultResponse(
    Guid SessionId,
    string Status,
    string? EndReason,
    int? Grade,
    DateTimeOffset StartedAt,
    DateTimeOffset? EndedAt);
