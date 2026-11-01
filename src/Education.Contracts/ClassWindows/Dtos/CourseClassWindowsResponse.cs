namespace Education.Contracts.ClassWindows;

/// <summary>Режим курса, группы и студенты для запуска пар, сами пары.</summary>
public sealed record CourseClassWindowsResponse(
    bool PracticeOnlyDuringClass,
    IReadOnlyList<string> Groups,
    IReadOnlyList<ClassWindowStudentOptionResponse> Students,
    IReadOnlyList<ClassWindowResponse> Windows);
