namespace Education.Application.ClassWindows;

/// <summary>Часть указанных при запуске пары студентов не назначена на курс.</summary>
public sealed class ClassWindowStudentsNotAssignedException() : Exception("Часть студентов не назначена на курс.");
