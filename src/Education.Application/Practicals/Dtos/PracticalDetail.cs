using Education.Domain.Practicals;

namespace Education.Application.Practicals;

/// <summary>Детали практики: вид, лимиты и привязка к внешнему модулю (если <see cref="PracticalKind.External"/>).</summary>
public sealed record PracticalDetail(
    Guid Id,
    string Name,
    PracticalKind Kind,
    bool IsPublic,
    int TriesCount,
    int? TimeLimitMinutes,
    ExternalModuleBinding? ModuleBinding,
    int? AttemptsPerSession = null);
