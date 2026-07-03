namespace Education.Domain.Tests;

/// <summary>
/// Поддерживаемый вид вопроса теста.
/// </summary>
public enum QuestionKind
{
    /// <summary>
    /// Вопрос с одним правильным вариантом.
    /// </summary>
    SingleChoice = 1,

    /// <summary>
    /// Вопрос с несколькими правильными вариантами.
    /// </summary>
    MultipleChoice = 2,

    /// <summary>
    /// Вопрос на сопоставление.
    /// </summary>
    Match = 3,

    /// <summary>
    /// Вопрос с коротким текстовым ответом.
    /// </summary>
    ShortAnswer = 4,
}

