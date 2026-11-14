namespace Education.Application.ClassWindows;

/// <summary>
/// Ошибка выбора пары, которая относится к другому курсу.
/// </summary>
public sealed class ClassWindowCourseMismatchException()
    : Exception("Выбранная пара относится к другому курсу.");
