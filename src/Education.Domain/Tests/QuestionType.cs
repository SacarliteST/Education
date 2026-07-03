using Education.Domain.Common;

namespace Education.Domain.Tests;

/// <summary>
/// Тип вопроса теста.
/// </summary>
public sealed class QuestionType : Entity
{
    /// <summary>
    /// Название типа вопроса.
    /// </summary>
    public string Name { get; private set; } = String.Empty;

    private QuestionType()
    {
    }

    /// <summary>
    /// Создает тип вопроса с фиксированным идентификатором.
    /// </summary>
    /// <param name="id">Идентификатор типа вопроса.</param>
    /// <param name="name">Название типа вопроса.</param>
    public QuestionType(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}

