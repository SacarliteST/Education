namespace Education.Contracts.Practicals;

/// <summary>Детали практики: вид, лимиты и привязка к внешнему модулю.</summary>
public sealed record PracticalDetailResponse(
    Guid Id,
    string Name,
    string Kind,
    bool IsPublic,
    int TriesCount,
    int? TimeLimitMinutes,
    ExternalModuleBindingResponse? ModuleBinding,
    int? AttemptsPerSession = null);
