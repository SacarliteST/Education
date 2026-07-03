namespace Education.Domain.Tests;

/// <summary>
/// Предопределенные идентификаторы типов вопросов.
/// </summary>
public static class QuestionTypeIds
{
    /// <summary>
    /// Идентификатор вопроса с одним правильным вариантом.
    /// </summary>
    public static readonly Guid SingleChoice = Guid.Parse("20000000-0000-0000-0000-000000000001");

    /// <summary>
    /// Идентификатор вопроса с несколькими правильными вариантами.
    /// </summary>
    public static readonly Guid MultipleChoice = Guid.Parse("20000000-0000-0000-0000-000000000002");

    /// <summary>
    /// Идентификатор вопроса на сопоставление.
    /// </summary>
    public static readonly Guid Match = Guid.Parse("20000000-0000-0000-0000-000000000003");

    /// <summary>
    /// Идентификатор вопроса с коротким текстовым ответом.
    /// </summary>
    public static readonly Guid ShortAnswer = Guid.Parse("20000000-0000-0000-0000-000000000004");

    /// <summary>
    /// Преобразует идентификатор типа вопроса в доменный вид вопроса.
    /// </summary>
    /// <param name="id">Идентификатор типа вопроса.</param>
    /// <returns>Доменный вид вопроса.</returns>
    public static QuestionKind ToKind(Guid id)
    {
        if (id == SingleChoice)
        {
            return QuestionKind.SingleChoice;
        }

        if (id == MultipleChoice)
        {
            return QuestionKind.MultipleChoice;
        }

        if (id == Match)
        {
            return QuestionKind.Match;
        }

        if (id == ShortAnswer)
        {
            return QuestionKind.ShortAnswer;
        }

        throw new ArgumentOutOfRangeException(nameof(id), id, "Unknown question type identifier.");
    }
}

