namespace Education.Contracts.Practicals;

/// <summary>Детали практики: вид, лимиты и привязка к внешнему модулю.</summary>
public sealed record PracticalDetailResponse(
    Guid Id,
    string Name,
    string Kind,
    bool IsPublic,
    int TriesCount,
    int? TimeLimitMinutes,
    ExternalModuleBindingResponse? ModuleBinding);

/// <summary>Привязка внешней практики к заданию модуля.</summary>
public sealed record ExternalModuleBindingResponse(
    Guid PracticalModuleId,
    string PracticalModuleSlug,
    string PracticalModuleName,
    Guid TaskId,
    string ExternalTaskRef);
