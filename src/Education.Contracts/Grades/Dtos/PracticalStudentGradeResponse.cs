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
