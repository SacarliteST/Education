namespace Education.Application.Grades;

/// <summary>Практика в ведомости курса.</summary>
public sealed record GradebookPractical(Guid Id, string Name, bool IsExternal, Guid ModuleId, string ModuleName);
