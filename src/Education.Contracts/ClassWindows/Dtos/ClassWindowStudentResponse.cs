namespace Education.Contracts.ClassWindows;

/// <summary>Студент, которому открыта пара точечно.</summary>
public sealed record ClassWindowStudentResponse(Guid UserId, string DisplayName);
