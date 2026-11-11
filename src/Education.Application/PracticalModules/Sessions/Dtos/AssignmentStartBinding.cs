namespace Education.Application.PracticalModules;

/// <summary>
/// Данные внешней практики для старта сессии по назначению случайного набора.
/// </summary>
/// <param name="ModuleId">Учебный модуль курса (для адреса возврата).</param>
/// <param name="CourseId">Курс практики.</param>
/// <param name="TriesCount">Лимит попыток на задание.</param>
/// <param name="TimeLimitMinutes">Лимит времени сессии в минутах или <see langword="null"/>.</param>
/// <param name="AttemptsPerSession">Лимит отправок решения внутри сессии или <see langword="null"/>.</param>
public sealed record AssignmentStartBinding(
    Guid ModuleId,
    Guid CourseId,
    int TriesCount,
    int? TimeLimitMinutes,
    int? AttemptsPerSession);
