namespace Education.Application.ClassWindows;

/// <summary>У студента не указана группа — начать пару для него невозможно.</summary>
public sealed class ClassWindowGroupRequiredException() : Exception("Группа не указана.");
