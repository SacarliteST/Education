namespace Education.Contracts.Grades;

/// <summary>Ведомость курса: практики и студенты с оценками.</summary>
public sealed record CourseGradebookResponse(
    IReadOnlyList<GradebookPracticalResponse> Practicals,
    IReadOnlyList<GradebookStudentResponse> Students);
