namespace Education.Contracts.Grades;

/// <summary>Оценка студента по практике для преподавателя.</summary>
/// <param name="StudentId">Идентификатор студента в учебной системе.</param>
/// <param name="StudentName">Полное имя студента.</param>
/// <param name="Login">Логин студента.</param>
/// <param name="Group">Учебная группа или <c>null</c>.</param>
/// <param name="Grade">Оценка 2–5 или <c>null</c>, если условия практики ещё не выполнены.</param>
/// <param name="Status"><c>passed</c> — зачтено (не ниже 3), <c>failed</c> — ниже проходной, <c>incomplete</c> — оценки ещё нет.</param>
/// <param name="Messages">Что осталось выполнить, если оценки ещё нет.</param>
public sealed record PracticalStudentGradeResponse(
    Guid StudentId,
    string StudentName,
    string Login,
    string? Group,
    int? Grade,
    string Status,
    IReadOnlyList<string> Messages);

/// <summary>Практика в ведомости курса.</summary>
public sealed record GradebookPracticalResponse(Guid Id, string Name, bool IsExternal, Guid ModuleId, string ModuleName);

/// <summary>Ячейка ведомости: студент × практика.</summary>
/// <param name="PracticalId">Практика.</param>
/// <param name="Assigned"><c>false</c> — практика студенту не назначена.</param>
/// <param name="Grade">Оценка 2–5 или <c>null</c>.</param>
/// <param name="Status"><c>passed</c> / <c>failed</c> / <c>incomplete</c>.</param>
public sealed record GradebookCellResponse(Guid PracticalId, bool Assigned, int? Grade, string Status);

/// <summary>Строка ведомости — студент и его ячейки в порядке практик ведомости.</summary>
public sealed record GradebookStudentResponse(
    Guid StudentId,
    string StudentName,
    string Login,
    string? Group,
    IReadOnlyList<GradebookCellResponse> Cells,
    int AssignedCount,
    int PassedCount,
    double? AverageGrade);

/// <summary>Ведомость курса: практики и студенты с оценками.</summary>
public sealed record CourseGradebookResponse(
    IReadOnlyList<GradebookPracticalResponse> Practicals,
    IReadOnlyList<GradebookStudentResponse> Students);
