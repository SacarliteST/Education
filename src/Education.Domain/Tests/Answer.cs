using Education.Domain.Common;

namespace Education.Domain.Tests;

/// <summary>
/// Ответ пользователя на вопрос в рамках результата теста.
/// </summary>
public sealed class Answer : Entity
{
    /// <summary>
    /// Сохраненное значение ответа пользователя.
    /// </summary>
    public string Answers { get; private set; } = String.Empty;

    /// <summary>
    /// Идентификатор связи практического материала с вопросом.
    /// </summary>
    public Guid PracticalMaterialBindQuestionId { get; private set; }

    /// <summary>
    /// Связь практического материала с вопросом.
    /// </summary>
    public PracticalMaterialBindQuestion PracticalMaterialBindQuestion { get; private set; } = null!;

    /// <summary>
    /// Идентификатор результата теста.
    /// </summary>
    public Guid TestResultId { get; private set; }

    /// <summary>
    /// Результат теста, к которому относится ответ.
    /// </summary>
    public TestResult TestResult { get; private set; } = null!;

    private Answer()
    {
    }

    /// <summary>
    /// Создает ответ пользователя.
    /// </summary>
    /// <param name="practicalMaterialBindQuestionId">Идентификатор связи практического материала с вопросом.</param>
    /// <param name="testResultId">Идентификатор результата теста.</param>
    /// <param name="answers">Сохраненное значение ответа пользователя.</param>
    public Answer(Guid practicalMaterialBindQuestionId, Guid testResultId, string answers)
    {
        PracticalMaterialBindQuestionId = practicalMaterialBindQuestionId;
        TestResultId = testResultId;
        Answers = answers;
    }
}

