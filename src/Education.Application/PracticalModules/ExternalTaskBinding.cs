namespace Education.Application.PracticalModules;

/// <summary>Данные привязки внешней практики: какой модуль, задание, лимиты и место практики в дереве курса.</summary>
public sealed record ExternalTaskBinding(
    Guid PracticalModuleId,
    string ExternalTaskRef,
    int TriesCount,
    int? TimeLimitMinutes,
    Guid ModuleId,
    Guid CourseId);
