namespace Education.Contracts.ClassWindows;

/// <summary>Доступ студента к практике курса.</summary>
public sealed record ClassAccessResponse(bool PracticeOnlyDuringClass, bool IsOpen, DateTimeOffset? EndsAt);
