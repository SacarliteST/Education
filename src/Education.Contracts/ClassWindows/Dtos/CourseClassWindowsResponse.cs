namespace Education.Contracts.ClassWindows;

/// <summary>Режим курса, группы студентов и пары.</summary>
public sealed record CourseClassWindowsResponse(
    bool PracticeOnlyDuringClass,
    IReadOnlyList<string> Groups,
    IReadOnlyList<ClassWindowResponse> Windows);
