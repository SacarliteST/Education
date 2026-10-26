namespace Education.Application.PracticalModules;

/// <summary>Данные для гейта кнопок на странице практики.</summary>
/// <param name="Session">Последняя сессия студента по заданию или <see langword="null"/>.</param>
/// <param name="AttemptsCount">Сколько заходов (сессий) уже использовано.</param>
/// <param name="TriesCount">Сколько заходов разрешено.</param>
/// <param name="TimeLimitMinutes">Лимит времени на заход, минуты, или <see langword="null"/>.</param>
/// <param name="BestGrade">Лучшая оценка по завершённым заходам.</param>
/// <param name="AttemptsPerSession">Лимит отправок решения в одном заходе или <see langword="null"/> — без лимита.</param>
/// <param name="SubmissionsCount">Сколько отправок записано по последней сессии (по журналу событий, с небольшой задержкой).</param>
public sealed record CurrentModuleSessionInfo(
    ModuleSessionStatus? Session,
    int AttemptsCount,
    int TriesCount,
    int? TimeLimitMinutes,
    int? BestGrade,
    int? AttemptsPerSession = null,
    int SubmissionsCount = 0);
