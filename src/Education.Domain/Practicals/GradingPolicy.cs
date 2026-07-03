namespace Education.Domain.Practicals;

/// <summary>
/// Политика перевода процента выполнения в оценку.
/// </summary>
public static class GradingPolicy
{
    /// <summary>
    /// Возвращает оценку по набранному и максимальному баллам.
    /// </summary>
    /// <param name="score">Набранный балл.</param>
    /// <param name="maxScore">Максимальный балл.</param>
    /// <param name="five">Порог оценки 5 в процентах.</param>
    /// <param name="four">Порог оценки 4 в процентах.</param>
    /// <param name="three">Порог оценки 3 в процентах.</param>
    /// <returns>Оценка от 2 до 5 или 0, если результат не может быть рассчитан.</returns>
    public static int GetGrade(double? score, double? maxScore, double five, double four, double three)
    {
        if (score is null || maxScore is null || maxScore == 0)
        {
            return 0;
        }

        var percent = score / maxScore;
        if (percent >= five / 100)
        {
            return 5;
        }

        if (percent >= four / 100)
        {
            return 4;
        }

        if (percent >= three / 100)
        {
            return 3;
        }

        return 2;
    }
}

