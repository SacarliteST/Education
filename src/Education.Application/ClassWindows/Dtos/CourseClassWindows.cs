namespace Education.Application.ClassWindows;

/// <summary>Режим курса, группы студентов и пары (для вкладки «Пары»).</summary>
public sealed record CourseClassWindows(
    bool PracticeOnlyDuringClass,
    IReadOnlyList<string> Groups,
    IReadOnlyList<ClassWindowInfo> Windows);
