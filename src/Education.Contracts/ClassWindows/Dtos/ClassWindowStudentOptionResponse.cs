namespace Education.Contracts.ClassWindows;

/// <summary>Студент курса, которому преподаватель может открыть пару точечно.</summary>
public sealed record ClassWindowStudentOptionResponse(Guid UserId, string DisplayName, string? GroupName);
