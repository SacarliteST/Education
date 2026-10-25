namespace Education.Contracts.Grades;

/// <summary>Практика в ведомости курса.</summary>
public sealed record GradebookPracticalResponse(Guid Id, string Name, bool IsExternal, Guid ModuleId, string ModuleName);
