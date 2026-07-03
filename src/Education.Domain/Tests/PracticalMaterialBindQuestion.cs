using Education.Domain.Common;
using Education.Domain.Practicals;

namespace Education.Domain.Tests;

/// <summary>
/// Связь практического материала с вопросом теста.
/// </summary>
public sealed class PracticalMaterialBindQuestion : Entity
{
    /// <summary>
    /// Идентификатор вопроса.
    /// </summary>
    public Guid QuestionId { get; private set; }

    /// <summary>
    /// Вопрос теста.
    /// </summary>
    public Question Question { get; private set; } = null!;

    /// <summary>
    /// Идентификатор практического материала.
    /// </summary>
    public Guid PracticalMaterialId { get; private set; }

    /// <summary>
    /// Практический материал.
    /// </summary>
    public PracticalMaterial PracticalMaterial { get; private set; } = null!;

    /// <summary>
    /// Ответы пользователей на этот вопрос в рамках практического материала.
    /// </summary>
    public List<Answer> Answers { get; private set; } = [];

    private PracticalMaterialBindQuestion()
    {
    }

    /// <summary>
    /// Создает связь практического материала с вопросом.
    /// </summary>
    /// <param name="practicalMaterialId">Идентификатор практического материала.</param>
    /// <param name="questionId">Идентификатор вопроса.</param>
    public PracticalMaterialBindQuestion(Guid practicalMaterialId, Guid questionId)
    {
        PracticalMaterialId = practicalMaterialId;
        QuestionId = questionId;
    }
}

