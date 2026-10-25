namespace Education.Contracts.ClassWindows;

/// <summary>Запрос смены режима курса.</summary>
public sealed record SetPracticeAccessRequest(bool PracticeOnlyDuringClass);
