namespace Education.Application.Grades;

/// <summary>Ведомость курса: практики и студенты с оценками.</summary>
public sealed record Gradebook(IReadOnlyList<GradebookPractical> Practicals, IReadOnlyList<GradebookStudent> Students);
