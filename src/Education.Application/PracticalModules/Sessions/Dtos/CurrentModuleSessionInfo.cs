namespace Education.Application.PracticalModules;

/// <summary>Данные для гейта кнопок на странице практики.</summary>
public sealed record CurrentModuleSessionInfo(
    ModuleSessionStatus? Session,
    int AttemptsCount,
    int TriesCount,
    int? TimeLimitMinutes,
    int? BestGrade);
