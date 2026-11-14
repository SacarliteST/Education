namespace Education.Application.Grades;

/// <summary>Студент не назначен на курс практического материала.</summary>
public sealed class StudentNotAssignedToCourseException()
    : Exception("Студент не назначен на курс практического материала.");
