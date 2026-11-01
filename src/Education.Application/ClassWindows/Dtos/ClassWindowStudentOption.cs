namespace Education.Application.ClassWindows;

/// <summary>Студент курса, которому преподаватель может открыть пару точечно.</summary>
public sealed record ClassWindowStudentOption(Guid UserId, string DisplayName, string? GroupName);
