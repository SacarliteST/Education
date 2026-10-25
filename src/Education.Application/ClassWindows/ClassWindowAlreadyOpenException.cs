namespace Education.Application.ClassWindows;

/// <summary>Для курса и группы уже идёт пара.</summary>
public sealed class ClassWindowAlreadyOpenException() : Exception("Для этой группы пара уже идёт.");
