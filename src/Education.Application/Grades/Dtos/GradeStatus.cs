namespace Education.Application.Grades;

/// <summary>Итог практики для преподавателя.</summary>
public enum GradeStatus
{
    /// <summary>Оценки ещё нет: студент не выполнил условия практики.</summary>
    Incomplete,

    /// <summary>Оценка получена, но ниже проходной.</summary>
    Failed,

    /// <summary>Зачтено: оценка не ниже проходной.</summary>
    Passed,
}
