namespace Education.Application.ClassWindows;

/// <summary>У курса уже идёт пара с пересекающейся аудиторией (та же группа или те же студенты).</summary>
public sealed class ClassWindowAlreadyOpenException() : Exception("Для этой группы или части студентов пара уже идёт.");
