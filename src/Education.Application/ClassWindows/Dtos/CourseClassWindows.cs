namespace Education.Application.ClassWindows;

/// <summary>Режим курса, группы и студенты для запуска пар, сами пары (для вкладки «Пары»).</summary>
public sealed record CourseClassWindows(
    bool PracticeOnlyDuringClass,
    IReadOnlyList<string> Groups,
    IReadOnlyList<ClassWindowStudentOption> Students,
    IReadOnlyList<ClassWindowInfo> Windows);
