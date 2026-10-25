using Education.Domain.Practicals;

namespace Education.Application.Grades;

/// <summary>
/// Единое правило итоговой оценки практики (шкала 2–5): им пользуются и студент (своя оценка),
/// и преподаватель (оценки студентов), чтобы цифры не расходились.
/// </summary>
public static class PracticalGradeCalculator
{
    /// <summary>
    /// Внутренняя практика: нужны все принятые задания и хотя бы одна сданная попытка теста.
    /// Оценка — лучший тест, а при наличии заданий — округлённое вверх среднее лучшего теста и средней оценки заданий.
    /// </summary>
    public static PracticalGrade Internal(
        int totalTasks,
        int acceptedTasks,
        IReadOnlyCollection<int> completedTestGrades,
        IReadOnlyCollection<int> taskGrades)
    {
        var messages = new List<string>();
        if (totalTasks != acceptedTasks)
        {
            messages.Add("Выполните все задания");
        }

        if (completedTestGrades.Count == 0)
        {
            messages.Add("Пройдите тестирование");
        }

        if (messages.Count != 0)
        {
            return new PracticalGrade(null, messages);
        }

        var bestTestGrade = completedTestGrades.Max();
        var grade = taskGrades.Count == 0
            ? bestTestGrade
            : (int)Math.Ceiling((bestTestGrade + taskGrades.Average()) / 2);

        return new PracticalGrade(grade, []);
    }

    /// <summary>
    /// Внешняя практика: балл модуля 0..100 переводится в оценку теми же порогами, что и обычные тесты практики
    /// (иначе внешняя и внутренняя практика показывали бы оценку в несовместимых шкалах).
    /// </summary>
    public static PracticalGrade External(PracticalMaterial practical, int? bestScore)
    {
        return bestScore is null
            ? new PracticalGrade(null, ["Пройдите практику"])
            : new PracticalGrade(practical.CalculateTestGrade(bestScore, 100), []);
    }

    /// <summary>Зачтено — оценка не ниже 3.</summary>
    public const int PassingGrade = 3;

    /// <summary>Определяет статус по оценке: <see langword="null"/> — не выполнена, не ниже <see cref="PassingGrade"/> — зачтена.</summary>
    /// <param name="grade">Оценка или <see langword="null"/>, если практика не выполнена.</param>
    /// <returns>Статус оценки.</returns>
    public static GradeStatus StatusOf(int? grade) => grade switch
    {
        null => GradeStatus.Incomplete,
        >= PassingGrade => GradeStatus.Passed,
        _ => GradeStatus.Failed,
    };
}
