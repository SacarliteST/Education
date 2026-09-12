namespace Education.Application.Practicals;

/// <summary>Сумма весов выбранных для теста вопросов превышает допустимый максимум в 100.</summary>
public sealed class PracticalQuestionsWeightExceededException(double totalWeight)
    : Exception($"Сумма весов выбранных вопросов ({totalWeight:0.##}) превышает 100.")
{
    /// <summary>Фактическая сумма весов выбранных вопросов.</summary>
    public double TotalWeight { get; } = totalWeight;
}
