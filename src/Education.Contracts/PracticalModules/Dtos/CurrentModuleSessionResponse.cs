using System.Text.Json;

namespace Education.Contracts.PracticalModules;

/// <summary>Данные гейта кнопок «Начать»/«Продолжить»/«Прервать».</summary>
public sealed record CurrentModuleSessionResponse(
    ModuleSessionResponse? Session,
    int AttemptsCount,
    int TriesCount,
    int? TimeLimitMinutes,
    int? BestGrade);
